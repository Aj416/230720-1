using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Notification;
using Tigerspike.Solv.Core.Extensions;
using System.IO;
using Tigerspike.Solv.Infra.Storage.Interfaces;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Services
{
	public class BrandService : IBrandService
	{
		private readonly IMapper _mapper;
		private readonly IPaymentService _paymentService;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IClientRepository _clientRepository;
		private readonly ITicketEscalationConfigRepository _escalationConfigRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IUploaderService _uploaderService;
		private readonly ITimestampService _timestampService;
		private readonly IMediatorHandler _mediator;
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly StorageOptions _storageOptions;
		private readonly IChatService _chatService;


		public BrandService(
			IChatService chatService,
			IMapper mapper,
			IMediatorHandler mediator,
			IPaymentService paymentService,
			IBillingDetailsRepository billingDetailsRepository,
			IBrandRepository brandRepository,
			ICachedBrandRepository cachedBrandRepository,
			IClientRepository clientRepository,
			ITicketEscalationConfigRepository escalationConfigRepository,
			IAdvocateRepository advocateRepository,
			IUploaderService uploaderService,
			ITimestampService timestampService,
			IBus bus, Microsoft.Extensions.Options.IOptions<BusOptions> busOptions,
			Microsoft.Extensions.Options.IOptions<StorageOptions> storageOptions)
		{
			_chatService = chatService;
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
			_mediator = mediator ??
				throw new ArgumentNullException(nameof(mediator));
			_brandRepository = brandRepository ??
				throw new ArgumentNullException(nameof(brandRepository));
			_cachedBrandRepository = cachedBrandRepository ??
				throw new ArgumentNullException(nameof(cachedBrandRepository));
			_clientRepository = clientRepository ??
				throw new ArgumentNullException(nameof(clientRepository));
			_escalationConfigRepository = escalationConfigRepository;
			_advocateRepository = advocateRepository ??
				throw new ArgumentNullException(nameof(advocateRepository));
			_uploaderService = uploaderService;
			_timestampService = timestampService;
			_paymentService = paymentService ??
				throw new ArgumentNullException(nameof(paymentService));
			_billingDetailsRepository = billingDetailsRepository;
			_bus = bus;
			_busOptions = busOptions.Value;
			_storageOptions = storageOptions.Value;
		}

		/// <inheritdoc/>
		public async Task<List<BrandModel>> GetAll(bool includePractice = false)
		{
			var query = _brandRepository.Queryable();

			if (!includePractice)
			{
				query = query.Where(b => b.IsPractice == false);
			}

			var brands = await query.ToListAsync();
			var models = _mapper.Map<List<BrandModel>>(brands);

			// include escalation flow info
			var brandsWithEscalationFlow = await _escalationConfigRepository.GetBrands();
			models
				.Where(x => brandsWithEscalationFlow.Contains(x.Id))
				.ToList()
				.ForEach(x => x.HasEscalationFlow = true);

			return models;
		}

		/// <inheritdoc/>
		public async Task<BrandModel> Get(Guid brandId)
		{
			var brand = await _brandRepository.FindAsync(brandId);
			var model = _mapper.Map<BrandModel>(brand);
			return model;
		}

		/// <inheritdoc/>
		public async Task<BrandPublicModel> GetPublicProfile(Guid brandId)
		{
			var brand = await _cachedBrandRepository.GetAsync(brandId);
			var model = _mapper.Map<BrandPublicModel>(brand);
			return model;
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<AdvocateBrandModel>> GetForAdvocate(Guid advocateId, bool? isPractice = null)
		{
			var brandList = await _brandRepository.GetBrands(advocateId, isPractice);
			var result = _mapper.Map<IEnumerable<AdvocateBrandModel>>(brandList);
			return result;
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<AbandonReasonModel>> GetAbandonReasons(Guid brandId)
		{
			var reasons = await _brandRepository.GetFirstOrDefaultAsync(
				predicate: x => x.Id == brandId,
				include: x => x.Include(i => i.AbandonReasons),
				selector: x => x.AbandonReasons
			);
			var activeReasons = reasons
				.Where(x => x.IsActive)
				.Where(x => x.IsForcedEscalation == false)
				.Where(x => x.IsBlockedAdvocate == false)
				.Where(x => x.IsAutoAbandoned == false)
				.ToList();

			return _mapper.Map<IEnumerable<AbandonReasonModel>>(activeReasons);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<TagModel>> GetTags(Guid brandId, bool activeOnly = true, TicketLevel? level = null)
		{
			var tags = await _brandRepository.GetTags(brandId, activeOnly, level);

			return _mapper.Map<IEnumerable<TagModel>>(tags.Where(x => !x.ParentTagId.HasValue));
		}

		/// <inheritdoc/>
		public async Task CreateAbandonReasons(Guid brandId, AbandonReasonModel[] abandonReasons)
		{
			foreach (var reason in abandonReasons)
			{
				await _mediator.SendCommand(new CreateAbandonReasonCommand(brandId, reason.Name, reason.Action));
			}
		}

		/// <inheritdoc/>
		public async Task CreateTags(CreateTagModel[] tags)
		{
			foreach (var tag in tags)
			{
				await _mediator.SendCommand(_mapper.Map<CreateTagCommand>(tag));
			}
		}

		/// <inheritdoc/>
		public async Task<Guid> GetClientBrandId(Guid clientId) => await _clientRepository.GetSingleOrDefaultAsync(x => x.BrandId, x => x.Id == clientId);

		/// <inheritdoc/>
		public async Task<BrandModel> GetClientBrand(Guid clientId)
		{
			var details = await _clientRepository.GetSingleOrDefaultAsync(
				selector: x => new
				{
					x.Brand,
					HasEscalationFlow = x.Brand.TicketEscalationConfigs.Count > 0
				},
				predicate: x => x.Id == clientId
			);

			var result = _mapper.Map<BrandModel>(details.Brand);
			result.HasEscalationFlow = details.HasEscalationFlow;
			return result;
		}

		public Task SetupPaymentAccount(Guid brandId, string billingAgreementToken) => _mediator.SendCommand(new UpdateBrandPaymentAccountCommand(brandId, billingAgreementToken));

		/// <inheritdoc/>
		public decimal CalculateTicketFee(decimal ticketPrice, decimal feePercentage) => Math.Round((ticketPrice / (1 - feePercentage)) - ticketPrice, 2);

		/// <inheritdoc/>
		public async Task SetTicketPrice(Guid brandId, decimal price, Guid userId) => await _mediator.SendCommand(new SetTicketPriceCommand(brandId, price, userId));

		/// <inheritdoc/>
		public Task Assign(Guid[] brandIds, Guid[] advocateApplicationIds) => _mediator.SendCommand(new AssignBrandsToAdvocateApplicationsCommand(brandIds, advocateApplicationIds));

		public async Task<string> GenerateBillingAgreementUrl(Guid brandId)
		{
			//TODO: to verify if we can retain the token for a while, and generate it only when it is expired,
			// rather than generating it every time we need the url.

			var (url, baToken) = await _paymentService.GenerateBillingAgreement(brandId);
			await _mediator.SendCommand(new UpdateBillingAgreementTokenCommand(brandId, baToken));

			return url;
		}

		/// <inheritdoc/>
		public async Task Apply(BrandApplicationModel model, string subject)
		{
			var platformDetails = await _billingDetailsRepository.GetCurrentForPlatform();
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = platformDetails.Email,
				Subject = subject,
				Template = EmailTemplates.BrandApplicationReceived.ToString(),
				Model = model.ToObjectDictionary()
			});
		}

		/// <inheritdoc/>
		public async Task<Guid?> Create(CreateBrandModel model) => await _mediator.SendCommand(_mapper.Map<CreateBrandCommand>(model));

		/// <inheritdoc/>
		public async Task<string[]> AddWhitelistPhrases(Guid brandId, string[] whitelistPhrases) =>
			await _chatService.AddWhitelistPhrases(brandId, whitelistPhrases);

		public async Task<BrandInductionModel> GetBrandInductionModel(Guid brandId)
		{
			var model = await _brandRepository.GetFirstOrDefaultAsync<BrandInductionModel>(_mapper, x => x.Id == brandId);
			return model ?? new BrandInductionModel();
		}

		/// <inheritdoc/>
		public async Task PostInductionSections(Guid brandId, BrandInductionModel model)
		{
			var currentModel = await GetBrandInductionModel(brandId);
			var currentSections = currentModel.Sections.ToList();
			var currentItems = currentSections.Where(x => x.Items != null).SelectMany(x => x.Items).ToList();

			var postSections = model.Sections.ToList();
			var postItems = postSections.Where(x => x.Items != null).SelectMany(x => x.Items).ToList();

			// remove
			var toRemoveItems = currentItems.Except(postItems, x => x.Id).ToList();
			foreach (var item in toRemoveItems)
			{
				var cmd = new DeleteInductionSectionItemCommand(brandId, item.Id);
				await _mediator.SendCommand(cmd);
			}

			var toRemoveSections = currentSections.Except(postSections, x => x.Id).ToList();
			foreach (var item in toRemoveSections)
			{
				var cmd = new DeleteInductionSectionCommand(brandId, item.Id);
				await _mediator.SendCommand(cmd);
			}

			// create
			var toCreateSections = postSections.Except(currentSections, x => x.Id).ToList();
			foreach (var item in toCreateSections)
			{
				var cmd = new CreateInductionSectionCommand(brandId, item.Id, item.Name, item.Order);
				await _mediator.SendCommand(cmd);
			}

			var toCreateItems = postItems.Except(currentItems, x => x.Id).ToList();
			foreach (var item in toCreateItems)
			{
				var section = postSections.Where(x => x.Items != null).Single(x => x.Items.Contains(item));
				var cmd = new CreateInductionSectionItemCommand(brandId, section.Id, item.Id, item.Name, item.Source, item.Order);
				await _mediator.SendCommand(cmd);
			}

			// update
			var toUpdateSections = postSections.Intersect(currentSections, x => x.Id).ToList();
			foreach (var item in toUpdateSections)
			{
				var cmd = new UpdateInductionSectionCommand(brandId, item.Id, item.Name, item.Order);
				await _mediator.SendCommand(cmd);
			}

			var toUpdateItems = postItems.Intersect(currentItems, x => x.Id).ToList();
			foreach (var item in toUpdateItems)
			{
				var section = postSections.Where(x => x.Items != null).Single(x => x.Items.Contains(item));
				var cmd = new UpdateInductionSectionItemCommand(brandId, section.Id, item.Id, item.Name, item.Source, item.Order);
				await _mediator.SendCommand(cmd);
			}

			await GetBrandInductionModel(brandId);
		}

		/// <inheritdoc/>
		public async Task CreateApiKey(Guid brandId, ApiKeyModel model) => await _mediator.SendCommand(new CreateApiKeyCommand(brandId, model.M2m, model.Sdk));

		/// <inheritdoc/>
		public async Task<ApiKeyModel> GenerateApiKey(Guid brandId)
		{
			var brandNameUrlFriendly = await GetBrandNameUrFriendly(brandId);
			return brandNameUrlFriendly != null ?
				new ApiKeyModel(
					await Nanoid.Nanoid.GenerateAsync(size: 32),
					GenerateApiKey(brandNameUrlFriendly, await Nanoid.Nanoid.GenerateAsync("0123456789", 15))
				) : null;
		}

		/// <inheritdoc/>
		public async Task<string> UploadAsset(Guid brandId, Stream stream, string assetName)
		{
			var brandName = await GetBrandNameUrFriendly(brandId);
			var uniqueAssetName = new DateTimeOffset(_timestampService.GetUtcTimestamp()).ToUnixTimeSeconds() + "-" + assetName;
			var uploadPath = Path.Combine(brandName, uniqueAssetName);

			await _uploaderService.Upload(stream, uploadPath, _storageOptions.BrandAssetsBucket);
			return string.Format(_storageOptions.BrandAssetsUrlFormat, brandName, uniqueAssetName.UrlEncode());
		}

		/// <inheritdoc/>
		public async Task<string> SetContract(Guid brandId, BrandContractModel model)
		{
			var contractUrl = await UploadContract(brandId, model.ContractContent);
			await _mediator.SendCommand(new SetBrandContractCommand(brandId, model.ContractTitle, contractUrl, model.BrandEmployeeCheck));
			return contractUrl;
		}

		/// <inheritdoc/>
		public async Task<string[]> DeleteWhitelistPhrase(Guid brandId, string[] whitelistPhrases) => await _chatService.RemoveWhitelistPhrases(brandId, whitelistPhrases);

		public async Task CreateQuiz(Guid brandId, QuizModel model) => await _mediator.SendCommand(new CreateQuizCommand(brandId, model.Title, model.Description, model.FailureMessage, model.SuccessMessage, model.AllowedMistakes.Value, model.Questions.Select(x => (x.Title, x.IsMultiChoice, x.Options.Select(y => (y.Text, y.Correct.Value)).ToList())).ToList()));
		/// <inheritdoc />
		public async Task<string> UploadTicketsImportToS3Bucket(Stream file, string uploadPath, string bucketName, string contentType, Dictionary<string, object> metaData) => await _uploaderService.Upload(file, uploadPath, contentType, bucketName, metaData: metaData);

		private async Task<string> UploadContract(Guid brandId, string content)
		{
			using var contentStream = await content.AsStream();
			return await UploadAsset(brandId, contentStream, "contract.txt");
		}

		private async Task<string> GetBrandNameUrFriendly(Guid brandId)
		{
			var brandName = await _brandRepository.GetFirstOrDefaultAsync(
				predicate: x => x.Id == brandId,
				selector: x => x.Name
			);

			return brandName.ToUrlFriendly();
		}

		private string GenerateApiKey(string brandNameUrlFriendly, string uniqueId) => $"{brandNameUrlFriendly}-{uniqueId}";

		/// <inheritdoc />
		public async Task<IEnumerable<CategoryModel>> GetCategories(Guid brandId, bool isEnabled = true)
		{
			var categories = await _brandRepository.GetCategories(brandId, isEnabled);

			return _mapper.Map<IEnumerable<CategoryModel>>(categories);
		}

		/// <inheritdoc />
		public async Task CreateCategories(CreateCategoryModel category, Guid brandId) => await _mediator.SendCommand(new CreateCategoryCommand(brandId, category.Categories.Select(c => (c.Name, c.Enabled ?? true, c.Order)).ToList()));

		/// <inheritdoc/>
		public async Task<bool> CheckCustomerTicketsEndpointEnabled(Guid brandId)
		{
			return await _brandRepository.Queryable()
					.Where(x => x.Id == brandId)
					.Select(x => x.EnableCustomerEndpoint)
					.FirstOrDefaultAsync();
		}
	}
}