using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Application.Models.Export;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Statistics;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Application.Services
{
	public class TicketService : ITicketService
	{
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediator;
		private readonly IJwtService _jwtService;
		private readonly IUserRepository _userRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly IClientRepository _clientRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly TicketLifecycleOptions _ticketLifecycleOptions;
		private readonly ITimestampService _timestampService;
		private readonly ITicketEscalationConfigRepository _ticketEscalationConfigRepository;
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly ICachedTicketRepository _cachedTicketRepository;
		private readonly IFeatureManager _featureManager;
		private readonly ITicketTagRepository _ticketTagRepository;
		private readonly ITicketImportRepository _ticketImportRepository;
		private readonly IBrandMetadataService _brandMetadataService;
		private readonly ICsvSerializer _csvSerializer;

		public TicketService(
			IOptions<TicketLifecycleOptions> ticketLifecycleOptions,
			ITimestampService timestampService,
			IInvoicingCycleRepository invoicingCycleRepository,
			ITicketRepository ticketRepository,
			IMapper mapper,
			IMediatorHandler mediator,
			IJwtService jwtService,
			IUserRepository userRepository,
			IAdvocateRepository advocateRepository,
			IClientRepository clientRepository,
			IBrandRepository brandRepository,
			ITicketEscalationConfigRepository ticketEscalationConfigRepository,
			ICachedBrandRepository cachedBrandRepository,
			ICachedTicketRepository cachedTicketRepository,
			ITicketImportRepository ticketImportRepository,
			IFeatureManager featureManager,
			ITicketTagRepository ticketTagRepository,
			IBrandMetadataService brandMetadataService,
			ICsvSerializer csvSerializer)
		{
			_ticketLifecycleOptions = ticketLifecycleOptions.Value;
			_timestampService = timestampService;
			_invoicingCycleRepository = invoicingCycleRepository;
			_ticketRepository = ticketRepository;
			_mapper = mapper;
			_mediator = mediator;
			_jwtService = jwtService;
			_userRepository = userRepository;
			_advocateRepository = advocateRepository;
			_clientRepository = clientRepository;
			_brandRepository = brandRepository;
			_ticketEscalationConfigRepository = ticketEscalationConfigRepository;
			_cachedBrandRepository = cachedBrandRepository;
			_cachedTicketRepository = cachedTicketRepository;
			_featureManager = featureManager;
			_ticketTagRepository = ticketTagRepository;
			_ticketImportRepository = ticketImportRepository;
			_brandMetadataService = brandMetadataService;
			_csvSerializer = csvSerializer;
		}

		private async Task<IEnumerable<TicketModel>> GetEscalatedDiagnosisEnabledTickets(Guid advocateId, TicketLevel userLevel)
		{
			var escalatedTickets = Enumerable.Empty<TicketModel>();
			if (userLevel == TicketLevel.Regular)
			{
				var advocate = await _advocateRepository.GetFirstOrDefaultAsync(
				predicate: a => a.Id == advocateId,
				include: inc => inc
					.Include(a => a.Brands)
					.ThenInclude(ab => ab.Brand)
					.ThenInclude(b => b.Tags)
				);

				var tagIds = advocate.Brands
					.SelectMany(ab => ab.Brand.Tags)?
					.Where(t => (t.DiagnosisEnabled ?? true) && t.Action == TicketFlowAction.Escalate)
					.Select(t => t.Id).ToHashSet();

				if (tagIds.Any())
				{
					var escalatedTicketIds = await _ticketTagRepository.GetEscalatedTicketIds(tagIds, advocateId);
					escalatedTickets = _mapper.Map<IEnumerable<TicketModel>>(await _ticketRepository.GetEscalatedDiagnosisEnabledTicket(t => escalatedTicketIds.Contains(t.Id)));
					escalatedTickets.ToList().ForEach(x => x.Status = TicketStatusEnum.Pending);
				}
			}

			return escalatedTickets;
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<TicketModel>> GetAdvocateTickets(Guid advocateId, TicketLevel userLevel)
		{
			var escalatedTickets = await GetEscalatedDiagnosisEnabledTickets(advocateId, userLevel);

			var advTickets = await _ticketRepository.GetAdvocateTickets(advocateId);

			await _brandMetadataService.FilterMetadata(advTickets);
			
			AssignStatusForUntaggedCustomerEndedTickets(advTickets.ToList());

			var tickets = _mapper.Map<IEnumerable<TicketModel>>(advTickets).ToList();

			var index = tickets.IndexOf(tickets.Where(t => t.Status == TicketStatusEnum.Closed).FirstOrDefault());

			tickets.InsertRange(index < default(int) ? tickets.Count : index, escalatedTickets);

			await CheckForDisabledTags(tickets, userLevel);

			return tickets;
		}

		public async Task<IEnumerable<TicketModel>> TrimSensitiveInformation(AccessLevel level, params TicketModel[] tickets)
		{
			foreach (var brandTickets in tickets.GroupBy(x => x.BrandId))
			{
				var brand = await _cachedBrandRepository.GetAsync(brandTickets.Key);

				foreach (var ticket in brandTickets)
				{

					foreach (var field in brand.FormFields)
					{
						if (field.AccessLevel > level)
						{
							ticket.Metadata.Remove(field.Name);
						}
					}

					if (level < AccessLevel.RegularSolver)
					{
						ticket.Price = 0;
					}

					if (level < AccessLevel.SuperSolver)
					{
						ticket.Customer.Email = null;
						ticket.Customer.Phone = null;
					}
				}
			}

			return tickets;
		}

		public async Task MarkResumption(Guid ticketId)
		{
			await _mediator.SendCommand(new SetNotificationResumptionStateCommand(ticketId, NotificationResumptionState.CustomerResumed));
			await _mediator.SendCommand(new SetCustomerRepeatedCountCommand(ticketId));
		}


		/// <inheritdoc/>
		public async Task<Guid?> Submit(CreateTicketModel ticketCreateModel, Guid brandId)
		{
			Guid? returningCustomerTicketId = null;
			var brand = await _cachedBrandRepository.GetAsync(brandId);
			var keyFields = brand.FormFields
				.Where(x => x.IsKey)
				.Select(x => x.Name)
				.ToList();

			if (keyFields.Any())
			{
				// find existing ticket first
				var keyMetadata = keyFields.ToDictionary(x => x, x => ticketCreateModel.Metadata.GetValueOrDefault(x));
				returningCustomerTicketId = await _ticketRepository.GetReturningCustomerTicketId(brandId, ticketCreateModel.Email, keyMetadata);
			}

			if (returningCustomerTicketId != null)
			{
				await _mediator.SendCommand(new SetReturningCustomerStateCommand(returningCustomerTicketId.Value, ReturningCustomerState.CustomerReturned));
				await _mediator.SendCommand(new SetCustomerRepeatedCountCommand(returningCustomerTicketId.Value));
				return returningCustomerTicketId.Value;
			}
			else
			{
				return await _mediator.SendCommand(
					new CreateTicketCommand(
						ticketCreateModel.FirstName,
						ticketCreateModel.LastName,
						ticketCreateModel.Email,
						ticketCreateModel.Question,
						brandId,
						transportType: ticketCreateModel.TransportType,
						referenceId: ticketCreateModel.ReferenceId,
						source: ticketCreateModel.Source,
						metadata: ticketCreateModel.Metadata,
						probingAnswers: ticketCreateModel.ProbingAnswers
					)
				);
			}
		}

		/// <inheritdoc/>
		public Task<Guid?> Reserve(Guid advocateId, bool isSuperSolver)
		{
			if (isSuperSolver)
			{
				return _mediator.SendCommand(new ReserveEscalatedTicketCommand(advocateId));
			}
			else
			{
				if (_featureManager.IsEnabledAsync(nameof(FeatureFlags.ByPassAcceptTicket)).Result)
				{
					return _mediator.SendCommand(new ReserveAndAcceptTicketCommand(advocateId));
				}
				else
				{
					return _mediator.SendCommand(new ReserveTicketCommand(advocateId));
				}
			}
		}

		/// <inheritdoc/>
		public Task SetTags(Guid ticketId, Guid[] tagIds, TicketLevel? userLevel) => _mediator.SendCommand(new SetTicketTagsCommand(ticketId, tagIds, userLevel));

		/// <inheritdoc/>
		public async Task<IEnumerable<TagModel>> GetTags(Guid ticketId, TicketLevel userLevel)
		{
			var tags = await _ticketRepository.GetTags(ticketId, userLevel);
			var tagIds = tags.Select(t => t.Id).ToHashSet();

			var result = tags.Where(x => !x.ParentTagId.HasValue).Select(t => new TagModel
			{
				Id = t.Id,
				Name = t.Name,
				Action = t.Action,
				SubTags = t.SubTags?.Where(st => tagIds.Contains(st.Id))
						.Select(r => new TagModel
						{
							Id = r.Id,
							Name = r.Name,
							Action = r.Action
						})?.ToList()
			}).ToList();

			return result;

		}

		/// <inheritdoc />
		public Task SetNps(Guid ticketId, int nps) => _mediator.SendCommand(new SetTicketNpsCommand(ticketId, nps));

		/// <inheritdoc/>
		public Task Accept(Guid ticketId) => _mediator.SendCommand(new AcceptTicketCommand(ticketId));

		/// <inheritdoc/>
		public async Task<string> Transition(Guid ticketId)
		{
			var ticket = await _ticketRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == ticketId,
				include: x => x
					.Include(i => i.Tags).ThenInclude(i => i.Tag)
					.Include(i => i.StatusHistory)
					.Include(i => i.Customer)
			);

			var message = string.Empty;

			if (ticket == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, "Ticket cannot be found."));
			}
			else
			{
				if (ticket.SposLead == true && ticket.SposEmailSent != true)
				{
					var sposNotification = ticket.Tags.All(x => x.Tag.SposNotificationEnabled == null || x.Tag.SposNotificationEnabled == true);
					await _mediator.RaiseEvent(new TicketSposLeadSetEvent(ticket.Id, ticket.Customer.Id, ticket.BrandId, sposNotification));
				}

				if (ticket.EscalationReason == null)
				{
					// get extra action associated with tag on ticket
					var tag = ticket.Tags
						.Select(x => new { x.Tag.Action, x.Tag.DiagnosisEnabled, x.Tag.Name, x.Tag.ParentTagId })
						.Where(x => x.Action.HasValue && x.ParentTagId is null)
						.FirstOrDefault();

					Command<Unit> cmd = tag?.Action switch
					{
						TicketFlowAction.Escalate => new EscalateTicketCommand(ticketId, TicketEscalationReason.Tag), // escalate, diagnosed to require Super Solver's assistance
						_ => new SolveTicketCommand(ticketId), // regular solve by regular solver
					};

					if (tag?.Action == TicketFlowAction.Escalate && tag?.DiagnosisEnabled != false)
					{
						message = "Your ticket has been escalated for confirmation. You will be eligible for payment when approved";
					}
					else
					{
						message = "Successfully sent close ticket request to the customer.";
					}

					await _mediator.SendCommand(cmd);
				}
				else
				{
					// solve by Super Solver
					await _mediator.SendCommand(new SolveTicketCommand(ticketId));
				}
			}

			return message;
		}

		/// <inheritdoc/>
		public Task Reopen(Guid ticketId) => _mediator.SendCommand(new ReopenTicketCommand(ticketId));

		/// <inheritdoc/>
		public Task Close(Guid ticketId, ClosedBy closedBy = ClosedBy.Customer) => _mediator.SendCommand(new CloseTicketCommand(ticketId, closedBy));

		/// <inheritdoc/>
		public Task Complete(Guid ticketId, TicketTagStatus tagStatus) => _mediator.SendCommand(new CompleteTicketCommand(ticketId, tagStatus));

		/// <inheritdoc/>
		public Task SetComplexity(Guid ticketId, int complexity) => _mediator.SendCommand(new SetTicketComplexityCommand(ticketId, complexity));

		/// <inheritdoc/>
		public Task SetCsat(Guid ticketId, int csat) => _mediator.SendCommand(new SetTicketCsatCommand(ticketId, csat));

		/// <inheritdoc/>
		public async Task<TicketModel> GetTicket(Guid ticketId, AccessLevel level)
		{
			var ticket = await _ticketRepository.GetFullTicket(t => t.Id == ticketId);
			
			await _brandMetadataService.FilterMetadata(ticket, level);

			var ticketModel = _mapper.Map<TicketModel>(ticket);
			
			if(ticket.Status == TicketStatusEnum.Closed && ticket.ClosedBy == ClosedBy.EndChat && ticket.TagStatus == null)
			{
				ticketModel.DisabledTags = await GetDisabledTagsOfTicketBrand(ticket);
				ticketModel.Status = TicketStatusEnum.EndedByCustomer;
				
				if(ticket.IsTaggingComplete())
				{
					//raise tag status change event.. enable the mark as solved/ tagging complete button accordingly
					await _mediator.RaiseEvent(new TicketTagsStatusChangedEvent(ticket.Id, ticket.BrandId, ticket.ReferenceId,
								ticket.ThreadId, ticket.AdvocateId, ticket.Advocate?.User?.FirstName, 
								ticket.Advocate?.GetCsat(ticket.BrandId), true));
				}
			}
			return ticketModel;
		}

		/// <inheritdoc/>
		public async Task<CustomerTicketModel> GetCustomerTicket(Guid ticketId)
		{
			// Customer should see limited set of data of a ticket.
			var ticket = await _ticketRepository.GetSingleOrDefaultAsync(
				selector: s => new CustomerTicketModel
				{
					Id = s.Id,
					BrandId = s.BrandId,
					Customer = new UserModel(s.Customer.Id, s.Customer.Email, s.Customer.FirstName, s.Customer.LastName, s.Customer.Phone),
					Question = s.Question,
					Status = s.Status,
					Csat = s.Csat,
					Nps = s.Nps,
					CreatedDate = s.CreatedDate,
					ModifiedDate = s.ModifiedDate,
					ClosedDate = s.ClosedDate,
					EscalatedDate = s.EscalatedDate,
					AdvocateTitle = s.Brand.AdvocateTitle,
					Advocate = s.AdvocateId.HasValue ? new CustomerAdvocateModel(s.AdvocateId.Value, s.Advocate.User.FirstName, s.Advocate.InternalAgent, s.Advocate.Brands.SingleOrDefault(b => b.BrandId == s.BrandId).Csat) : null,
					AllAdvocates = s.StatusHistory
						.Where(x => x.AdvocateId.HasValue)
						.Select(x => new CustomerAdvocateModel(x.Advocate.Id, x.Advocate.User.FirstName, x.Advocate.InternalAgent, null)),
					AdditionalFeedBack = s.AdditionalFeedBack
				},
				predicate: t => t.Id == ticketId);

			if (ShouldBeVisibleToCustomer(ticket))
			{
				ticket.AllAdvocates = ticket.AllAdvocates.DistinctBy(x => x.Id);
				return ticket;
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(nameof(GetTicket), "Ticket is no longer eligible to view for the customer", (int)HttpStatusCode.Gone));
				return null;
			}
		}

		private bool ShouldBeVisibleToCustomer(CustomerTicketModel ticket)
		{
			// A new escalation type has emerged, and Super Solver can pick up those escalated tickets (instead of sending them to the Client's)
			// So the customer should still see the ticket (its current status would not be Escalated anymore)
			var stillEscalated = ticket.Status == TicketStatusEnum.Escalated;
			var finalizationDate = ticket.ClosedDate ?? (stillEscalated ? ticket.EscalatedDate : (DateTime?)null);
			return finalizationDate == null || finalizationDate.Value.AddMinutes(_ticketLifecycleOptions.CustomerVisibilityAfterClosedInMinutes) > _timestampService.GetUtcTimestamp();
		}

		/// <inheritdoc />
		public async Task<bool> CanView(ClaimsPrincipal user, params Guid[] ticketIds)
		{
			var userId = user.GetId();

			if (!await _userRepository.ExistsAsync(u => u.Id == userId) || !ticketIds.Any())
			{
				return false;
			}

			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				return await _ticketRepository.ExistsAsync(t => ticketIds.Contains(t.Id) && t.AdvocateId == userId);
			}
			else if (user.IsInRole(SolvRoles.Customer))
			{
				return await _ticketRepository.ExistsAsync(t => ticketIds.Contains(t.Id) && t.Customer.Id == userId);
			}
			else if (user.IsInRole(SolvRoles.Client))
			{
				var brandId = await _clientRepository.GetSingleOrDefaultAsync(s => s.BrandId, p => p.Id == userId);
				return await _ticketRepository.ExistsAsync(t => ticketIds.Contains(t.Id) && t.BrandId == brandId);
			}
			else if (user.IsInRole(SolvRoles.Admin))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <inheritdoc />
		public async Task<bool> CanEdit(ClaimsPrincipal user, SolvOperationEnum operation, Guid[] ticketIds)
		{
			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				return await _ticketRepository.ExistsAsync(GetAdvocateRules(user.GetId(), operation, ticketIds));
			}

			if (user.IsInRole(SolvRoles.Customer))
			{
				return await _ticketRepository.ExistsAsync(GetCustomerRules(user.GetId(), operation, ticketIds));
			}

			return false;
		}

		private Expression<Func<Ticket, bool>> GetAdvocateRules(Guid userId, SolvOperationEnum operation, Guid[] ticketIds)
		{
			switch (operation)
			{
				case SolvOperationEnum.SendMessage:
					// Advocate can send a message only if the ticket is assigned to him/her.
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && t.Status == TicketStatusEnum.Assigned;
				case SolvOperationEnum.RejectTicket:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && t.Status == TicketStatusEnum.Reserved;
				case SolvOperationEnum.AbandonTicket:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && t.Status == TicketStatusEnum.Assigned;
				case SolvOperationEnum.RateComplexity:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && t.Status == TicketStatusEnum.Closed;
				case SolvOperationEnum.TransitionTicket:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && t.Status == TicketStatusEnum.Assigned;
				case SolvOperationEnum.SetTags:
				case SolvOperationEnum.SetSpos:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId;
				case SolvOperationEnum.SetCategory:
				case SolvOperationEnum.SetDiagnosis:
				case SolvOperationEnum.SetValidTransfer:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && (t.Status == TicketStatusEnum.Assigned || (t.Status == TicketStatusEnum.Closed && t.ClosedBy == ClosedBy.EndChat && t.TagStatus == null));
				case SolvOperationEnum.CompleteTicket:
					return t => ticketIds.Contains(t.Id) && t.AdvocateId == userId && t.Status == TicketStatusEnum.Closed && t.TagStatus == null;
				default:
					return t => false;
			}
		}

		private Expression<Func<Ticket, bool>> GetCustomerRules(Guid userId, SolvOperationEnum operation, Guid[] ticketIds)
		{
			switch (operation)
			{
				case SolvOperationEnum.SendMessage:
					// Customer can send a message only if the ticket is created by him/her and in assigned status.
					return t => ticketIds.Contains(t.Id) && t.Customer.Id == userId && t.Status == TicketStatusEnum.Assigned;
				case SolvOperationEnum.CloseTicket:
					// Customer can close a ticket only is created by him/her and in solved status.
					return t => ticketIds.Contains(t.Id) && t.Customer.Id == userId && t.Status == TicketStatusEnum.Solved;
				case SolvOperationEnum.ResumeChat:
					// Customer can resume chat on a ticket if ticket is allowed status
					return t => ticketIds.Contains(t.Id) && t.Customer.Id == userId &&
						(
							t.Status == TicketStatusEnum.Assigned ||
							t.Status == TicketStatusEnum.New ||
							t.Status == TicketStatusEnum.Reserved ||
							t.Status == TicketStatusEnum.Solved
						);
				default:
					return t => false;
			}
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<RejectReasonModel>> GetRejectReasons() => _mapper.Map<IEnumerable<RejectReasonModel>>(await _ticketRepository.GetRejectionReasons());

		/// <inheritdoc/>
		public async Task<TicketStatisticsPerformanceModel> GetStatisticsPerformanceOverview(Guid advocateId, Guid? brandId, DateTime? from = null)
		{
			return new TicketStatisticsPerformanceModel
			{
				AverageTimeToComplete = await _ticketRepository.GetAverageTimeToComplete(advocateId: advocateId, brandId: brandId, from: from) ?? 0,
				SuccessRate = await _ticketRepository.GetSuccessRate(advocateId: advocateId, brandId: brandId, from: from) ?? 0,
				AverageCsat = await _ticketRepository.GetAverageCsat(advocateId: advocateId, brandId: brandId, from: from) ?? 0,
			};
		}

		/// <inheritdoc/>
		public async Task<TicketStatisticsOverviewModel> GetStatisticsOverviewForAll(DateTime? from = null, DateTime? to = null)
		{
			return new TicketStatisticsOverviewModel
			{
				TotalPrice = await _ticketRepository.GetTotalPriceForAll(from, to),
				AverageCsat = await _ticketRepository.GetAverageCsat(from: from, to: to) ?? 0,
				SuccessRate = await _ticketRepository.GetSuccessRate(from: from, to: to) ?? 0,
				AveragePrice = await _ticketRepository.GetAveragePriceForAll(from, to) ?? 0,
				AverageComplexity = await _ticketRepository.GetAverageComplexityForAll(from, to) ?? 0,
				ClosedFirstRound = await _ticketRepository.GetFirstAttemptSuccessRateForAll(from, to) ?? 0,
				AverageTimeToComplete = await _ticketRepository.GetAverageTimeToComplete(from: from, to: to) ?? 0,
				AverageResponseTime = await _ticketRepository.GetAverageTimeToRespondForAll(from, to) ?? 0
			};
		}

		/// <inheritdoc/>
		public async Task<TicketStatisticsOverviewModel> GetStatisticsOverviewForBrand(Guid brandId, DateTime? from = null, DateTime? to = null)
		{
			return new TicketStatisticsOverviewModel
			{
				TotalPrice = (from.HasValue && to.HasValue) ? await _ticketRepository.GetTotalPriceForBrand(brandId, from.Value, to.Value) : await _ticketRepository.GetTotalPriceForBrand(brandId),
				AverageCsat = await _ticketRepository.GetAverageCsat(brandId: brandId, from: from, to: to) ?? 0,
				SuccessRate = await _ticketRepository.GetSuccessRate(brandId: brandId, from: from, to: to) ?? 0,
				AveragePrice = await _ticketRepository.GetAveragePriceForBrand(brandId, from, to) ?? 0,
				AverageComplexity = await _ticketRepository.GetAverageComplexityForBrand(brandId, from, to) ?? 0,
				ClosedFirstRound = await _ticketRepository.GetFirstAttemptSuccessRateForBrand(brandId, from, to) ?? 0,
				AverageTimeToComplete = await _ticketRepository.GetAverageTimeToComplete(brandId: brandId, from: from, to: to) ?? 0,
				AverageResponseTime = await _ticketRepository.GetAverageTimeToRespondForBrand(brandId, from, to) ?? 0,
			};
		}

		/// <inheritdoc/>
		public async Task<TicketStatisticsOverviewModel> GetStatisticsOverviewForAdvocate(Guid advocateId, DateTime? from = null, DateTime? to = null)
		{
			return new TicketStatisticsOverviewModel
			{
				TotalPrice = await _ticketRepository.GetTotalPriceForAdvocate(advocateId, from, to),
				AverageCsat = await _ticketRepository.GetAverageCsat(advocateId: advocateId, from: from, to: to) ?? 0,
				SuccessRate = await _ticketRepository.GetSuccessRate(advocateId: advocateId, from: from, to: to) ?? 0,
				AveragePrice = await _ticketRepository.GetAveragePriceForAdvocate(advocateId, from, to) ?? 0,
				AverageTimeToComplete = await _ticketRepository.GetAverageTimeToComplete(advocateId: advocateId, from: from, to: to) ?? 0,
			};
		}

		private TicketStatisticByStatusModel GetStatisticsByStatus(Dictionary<TicketStatusEnum, int> input)
		{
			return new TicketStatisticByStatusModel
			{
				Open = input.GetValueOrDefault(TicketStatusEnum.New) + input.GetValueOrDefault(TicketStatusEnum.Reserved),
				Closed = input.GetValueOrDefault(TicketStatusEnum.Closed),
				Solved = input.GetValueOrDefault(TicketStatusEnum.Solved),
				InProgress = input.GetValueOrDefault(TicketStatusEnum.Assigned),
			};
		}

		/// <inheritdoc />
		public async Task<TicketStatisticByStatusModel> GetStatisticsByStatusForAll() => GetStatisticsByStatus(await _ticketRepository.GetCountByStatusForAll());

		/// <inheritdoc />
		public async Task<TicketStatisticByStatusModel> GetStatisticsByStatusForBrand(Guid brandId) => GetStatisticsByStatus(await _ticketRepository.GetCountByStatusForBrand(brandId));

		/// <inheritdoc />
		public async Task<TicketStatisticByStatusModel> GetStatisticsByStatusForAdvocate(Guid advocateId) => GetStatisticsByStatus(await _ticketRepository.GetCountByStatusForAdvocate(advocateId));

		/// <inheritdoc/>
		public async Task<TicketStatisticsForBillingCycleModel> GetStatisticsForPeriod(Guid brandId, DateTime fromDate,
			DateTime toDate)
		{
			return new TicketStatisticsForBillingCycleModel
			{
				TicketCount = await _ticketRepository.CountAsync(x =>
					x.BrandId == brandId && x.Status == TicketStatusEnum.Closed && x.ClosedDate >= fromDate &&
					x.ClosedDate < toDate),
				TotalPrice = await _ticketRepository.GetTotalPriceForBrand(brandId, fromDate, toDate),
				IssueDate = toDate.Date,
			};
		}

		/// <inheritdoc/>
		public async Task<TicketStatisticsForBillingCycleModel> GetStatisticsForCurrentPeriod(Guid brandId)
		{
			DateTime? lastInvoicingCycleEndDate =
				await _invoicingCycleRepository.GetFirstOrDefaultAsync(selector: i => i.To, orderBy: o => o.OrderByDescending(i => i.From));

			var fromDate = lastInvoicingCycleEndDate ?? _timestampService.GetUtcTimestamp().StartOfYear();
			var endOfCurrentWeek = _timestampService.GetUtcTimestamp().StartOfWeek(DayOfWeek.Monday).AddDays(7);

			return new TicketStatisticsForBillingCycleModel
			{
				TicketCount = await _ticketRepository.CountAsync(x =>
					x.BrandId == brandId && x.Status == TicketStatusEnum.Closed && x.ClosedDate >= fromDate &&
					x.ClosedDate < endOfCurrentWeek),
				TotalPrice = await _ticketRepository.GetTotalPriceForBrand(brandId, fromDate, endOfCurrentWeek),
				IssueDate = endOfCurrentWeek.Date,
			};
		}

		/// <inheritdoc/>
		public async Task<TicketStatisticsForEscalatedModel> GetStatisticsForEscalated(Guid brandId, DateTime? from = null, DateTime? to = null)
		{
			var stats = await _ticketRepository.GetEscalatedBySource(brandId, from, to);
			return new TicketStatisticsForEscalatedModel
			{
				Items = stats.Select(x => new TicketStatisticsSourceCount(x.source ?? TicketSource.DefaultName, x.count)).ToList(),
				Total = stats.Sum(x => x.count)
			};
		}

		/// <inheritdoc/>
		public bool IsEscalationAbandonedThresholdReached(Ticket ticket, TicketEscalationConfig escalationConfig) =>
			ticket.AbandonedCount >= escalationConfig?.AbandonedCount;

		/// <inheritdoc/>
		public bool IsEscalationRejectionThresholdReached(Ticket ticket, TicketEscalationConfig escalationConfig) =>
			ticket.RejectionCount >= escalationConfig?.RejectionCount;

		/// <inheritdoc/>
		public bool IsEscalationTimeoutReached(Ticket ticket, TicketEscalationConfig escalationConfig)
		{
			return escalationConfig?.OpenTimeInSeconds != null &&
				ticket.CreatedDate.AddSeconds(escalationConfig.OpenTimeInSeconds.Value) < _timestampService.GetUtcTimestamp();
		}

		public async Task<TicketSearchModel> GetSearchModel(Guid ticketId)
		{
			TicketSearchModel result = null;
			var ticket = await _ticketRepository.GetFullTicket(t => t.Id == ticketId);
			if (ticket != null)
			{
				result = _mapper.Map<TicketSearchModel>(ticket);

				if (ticket.Status == TicketStatusEnum.Escalated)
				{
					// for escalated tickets, retrieve name of the Solver that touched the ticket as the last one
					// result.AdvocateFullName = await GetLastAdvocateFullName(ticket);
				}
			}

			return result;
		}

		/// <inheritdoc/>
		public bool ShouldTicketBeClosed(Ticket ticket)
		{
			var lastStatusHistory = ticket.StatusHistory.OrderByDescending(s => s.CreatedDate).FirstOrDefault();
			return
				ticket.Status == TicketStatusEnum.Solved &&
				ticket.IsPractice == false &&
				lastStatusHistory?.CreatedDate.AddMinutes(ticket.Brand.WaitMinutesToClose) <= _timestampService.GetUtcTimestamp();
		}

		/// <inheritdoc/>
		public async Task<Stream> GetExportData(TicketCsvExportParameterModel ticketCsvExportParameterModel)
		{
			switch (ticketCsvExportParameterModel.TriggeredBy)
			{
				case CsvExportSource.Admin:
					return await _csvSerializer.GetStream(await GetExportDataForAdmin(ticketCsvExportParameterModel));
				case CsvExportSource.Client:
					return await _csvSerializer.GetStream(await GetExportDataForClient(ticketCsvExportParameterModel));
				default:
					throw new NotImplementedException();
			}
		}

		/// <inheritdoc />
		public async Task Abandon(Guid ticketId, Guid[] reasonIds) => await _mediator.SendCommand(new AbandonTicketCommand(ticketId, reasonIds));

		/// <inheritdoc />
		public async Task Reject(Guid ticketId, int[] reasonIds) => await _mediator.SendCommand(new RejectTicketCommand(ticketId, reasonIds));

		/// <inheritdoc />
		public async Task Escalate(Guid ticketId)
		{
			var (statusBefore, brandId, question, isPractice, sourceId, sourceName, referenceId, advocateFirstName) =
				await _ticketRepository.GetFirstOrDefaultAsync(
					selector: x =>
						Tuple.Create(x.Status, x.BrandId, x.Question, x.IsPractice, x.SourceId,
								x.Source != null ? x.Source.Name : string.Empty, x.ReferenceId,
								x.Advocate.User.FirstName)
							.ToValueTuple(),
					predicate: x => x.Id == ticketId
				);

			if (statusBefore == TicketStatusEnum.Closed || statusBefore == TicketStatusEnum.Reserved)
			{
				await _mediator.RaiseEvent(new DomainNotification("ForcedEscalation", "Ticket can not escalated."));
			}
			else if (statusBefore == TicketStatusEnum.Escalated)
			{
				await _mediator.RaiseEvent(new DomainNotification("ForcedEscalation", "Ticket already escalated."));
			}
			else
			{
				var escalationConfig = await _ticketEscalationConfigRepository.Get(brandId, sourceId);

				if (escalationConfig != null)
				{
					await _mediator.SendCommand(new EscalateTicketCommand(ticketId,
						TicketEscalationReason.AdminEscalated));
				}
				else
				{
					var abandonReasons = await _brandRepository.GetFirstOrDefaultAsync(
						predicate: x => x.Id == brandId,
						selector: x => x.AbandonReasons
					);

					var forcedEscalationReasons = abandonReasons
						.Where(x => x.IsForcedEscalation)
						.Select(x => x.Id)
						.ToArray();

					await _mediator.SendCommand(new AbandonTicketCommand(ticketId, forcedEscalationReasons));
				}
			}
		}

		/// <inheritdoc/>
		public Task<AdvocateStatisticPeriodSummaryModel> GetStatisticsPeriodPackage(Guid advocateId) => _cachedTicketRepository.GetStatisticsPeriodPackage(advocateId);

		/// <inheritdoc/>
		public Task<int> GetAvailableTickets(Guid advocateId, TicketLevel level) => _cachedBrandRepository.GetAvailableTickets(advocateId, level);

		/// <inheritdoc/>
		public async Task<AdvocatePerformanceStatisticPeriodSummaryModel> GetAdvocatePerformanceStatisticsPeriod(Guid advocateId, string period, DateTime? time, Guid[] brandIds)
		{
			if (Enum.TryParse(period, true, out Period timeFrame))
			{
				var from = GetStartOfPeriod(timeFrame, time ?? DateTime.UtcNow);
				var to = GetEndOfPeriod(timeFrame, from);

				var advocateBrandIds = await _advocateRepository.GetBrandIdsForGraph(advocateId);

				return await _cachedTicketRepository.GetAdvocatePerformanceStatisticsPeriod(advocateId, from, to, brandIds, timeFrame, advocateBrandIds.ToArray());
			}
			else
			{
				throw new InvalidOperationException($"Cannot fetch advocate performance. Selected timeframe - {period} is invalid.");
			}
		}

		/// <inheritdoc/>
		public (TicketFlowAction? action, string value) GetProbingEvaluation(IEnumerable<ProbingResult> probingResults, ProbingForm probingForm)
		{
			var options = probingForm != null ?
				probingForm.Questions.SelectMany(x => x.Options).ToList() :
				probingResults.Select(x => x.ProbingQuestionOption).ToList();

			var flow = probingResults
				.Where(x => x.ProbingQuestionOptionId != null)
				.Select(x => options.FirstOrDefault(y => y.Id == x.ProbingQuestionOptionId))
				.Where(x => x.Action != null)
				.FirstOrDefault();

			return flow != null ? (flow.Action, flow.Value) : (null, null);
		}

		/// <inheritdoc/>
		public async Task<IPagedList<TicketImportModel>> GetImportTicket(Guid brandId, int pageIndex = 0,
			int pageSize = 25, TicketImportSortBy sortBy = TicketImportSortBy.uploadDate, SortOrder sortOrder = SortOrder.Desc)
		{
			return await _ticketImportRepository.GetPagedListAsync<TicketImportModel>(
				mapper: _mapper,
				predicate: x => x.BrandId == brandId,
				orderBy: x => x.OrderBy(sortBy, sortOrder),
				pageIndex: pageIndex,
				pageSize: pageSize);
		}

		/// <inheritdoc/>
		public async Task<string> GetAllFailureImportTicket(Guid ticketImportid)
		{
			var failureTickets = _ticketImportRepository.Queryable()
				.Include(x => x.Failures)
				.Where(x => x.Id == ticketImportid)
				.Select(x => x.Failures).FirstOrDefault();

			string csv;
			using (var sw = new StringWriter())
			{
				// Writing the header row
				foreach (var failureTicket in failureTickets)
				{
					// Write the application record
					await sw.WriteLineAsync(failureTicket.RawInput + "," + failureTicket.FailureReason);
				}

				csv = sw.ToString();
			}

			return csv;
		}

		/// <inheritdoc/>
		public async Task SetSpos(Guid ticketId, TicketSposModel model) => await _mediator.SendCommand(new SetTicketSposCommand(ticketId, model.SposLead, model.SposDetails));

		private DateTime GetStartOfPeriod(Period timeFrame, DateTime time)
		{
			// Checking and setting start period in BE if FE send wrong date
			switch (timeFrame)
			{
				case Period.Week:
					return time.StartOfWeek();
				case Period.Month:
					return time.StartOfMonth();
				case Period.Year:
					return time.StartOfYear();
				default:
					return time.StartOfWeek();
			}
		}

		private DateTime GetEndOfPeriod(Period timeFrame, DateTime from)
		{
			switch (timeFrame)
			{
				case Period.Week:
					return from.AddDays(7);
				case Period.Month:
					return from.AddDays(DateTime.DaysInMonth(from.Year, from.Month));
				case Period.Year:
					return from.AddDays(DateTime.IsLeapYear(from.Year) ? 366 : 365);
				default:
					return from.AddDays(7);
			}
		}

		/// <inheritdoc/>
		public async Task SetCategory(Guid ticketId, Guid categoryId, Guid advocateId) => await _mediator.SendCommand(new SetTicketCategoryCommand(ticketId, categoryId, advocateId));

		/// <inheritdoc/>
		public async Task<CategoryModel> GetCategory(Guid ticketId) => _mapper.Map<CategoryModel>(await _ticketRepository.GetCategory(ticketId));

		/// <inheritdoc/>
		public async Task<bool> SetDiagnosis(Guid ticketId, TicketDiagnosisModel model) => await _mediator.SendCommand(new SetTicketDiagnosisCommand(ticketId, model.CorrectlyDiagnosed));

		/// <inheritdoc/>
		public async Task<bool> SetValidTransfer(Guid ticketId, TicketValidTransferModel model) => await _mediator.SendCommand(new SetTicketValidTransferCommand(ticketId, model.IsValidTransfer));

		/// <inheritdoc/>
		public async Task SetAdditionalFeedBack(Guid ticketId, string additionalFeedBack) => await _mediator.SendCommand(new SetTicketAdditionalFeedBackCommand(ticketId, additionalFeedBack));

		/// <inheritdoc/>
		public async Task<List<Guid>> GetDisabledTagsOfTicketBrand(Ticket ticket)
		{
			var brandTags = await _brandRepository.GetTags(ticket.BrandId);
			List<Guid> brandDisableTags = new List<Guid>();

			if(ticket.Level == TicketLevel.Regular)
			{
				brandDisableTags = brandTags.Where(x => x.L1PostClosureDisable).Select(x => x.Id).ToList();
			}
			else if (ticket.Level == TicketLevel.SuperSolver)
			{
				brandDisableTags = brandTags.Where(x => x.L2PostClosureDisable).Select(x => x.Id).ToList();
			}

			return brandDisableTags;
		}

		/// <summary>
		/// A helper method to handle the untagged tickets that were closed/ended by the customer
		/// </summary>
		/// <returns></returns>
		private void AssignStatusForUntaggedCustomerEndedTickets(List<Ticket> tickets)
		{
			tickets.Where(x => x.Status == TicketStatusEnum.Closed && x.ClosedBy == ClosedBy.EndChat && x.TagStatus == null)
				.ToList()
				.ForEach(x =>
				{
					x.Status = TicketStatusEnum.EndedByCustomer;
				});
		}

		/// <summary>
		/// The method to check for disabled tags
		/// </summary>
		/// <param name="tickets"></param>
		/// <param name="userLevel"></param>
		private async Task CheckForDisabledTags(List<TicketModel> tickets, TicketLevel userLevel)
		{
			var endedByCustomerTickets = tickets.Where(x => x.Status == TicketStatusEnum.EndedByCustomer).ToList();

			if (!endedByCustomerTickets.Any())
			{
				return;
			}

			var brandTagModels = new List<BrandTagModel>();
			var brands = endedByCustomerTickets.Select(x => x.BrandId).Distinct().ToList();
			
			foreach(var brand in brands) 
			{
				var tags = await _brandRepository.GetTags(brand);
				List<Guid> disabledTags = new List<Guid>();

				if (userLevel == TicketLevel.Regular)
				{
					disabledTags = tags.Where(x => x.L1PostClosureDisable).Select(x => x.Id).ToList();
				}
				else if (userLevel == TicketLevel.SuperSolver)
				{
					disabledTags = tags.Where(x => x.L2PostClosureDisable).Select(x => x.Id).ToList();
				}

				brandTagModels.Add(new BrandTagModel() { BrandId = brand, DisabledTags = disabledTags});
			}

			tickets.ForEach(ticket =>
			{
				ticket.DisabledTags = ticket.Status == TicketStatusEnum.EndedByCustomer ? brandTagModels.Where(x => x.BrandId == ticket.BrandId).Select(x => x.DisabledTags).FirstOrDefault() ?? new List<Guid>() : new List<Guid>();
			});

		}

		/// <summary>
		/// The method to fetch admin ticket data for export
		/// </summary>
		/// <param name="ticketCsvExportParameterModel"></param>
		/// <returns></returns>
		private async Task<IList<TicketAdminExportModel>> GetExportDataForAdmin(TicketCsvExportParameterModel ticketCsvExportParameterModel)
		{
			var result = new List<TicketAdminExportModel>();
			var page = PagedList.ForLoop<Ticket>();

			do
			{
				// process data in small chunks, not to consume too much memory
				page = await _ticketRepository.GetPagedExportData(ticketCsvExportParameterModel.DateFrom,ticketCsvExportParameterModel.DateTo, ticketCsvExportParameterModel.BrandId, page.PageIndex + 1);
				var exportItems = page.Items.Select(x => 
				{
					if (x.Status == TicketStatusEnum.Closed && x.ClosedBy == ClosedBy.EndChat && x.TagStatus == null)
					{
						x.Status = TicketStatusEnum.EndedByCustomer;
					}
					return _mapper.Map<TicketAdminExportModel>(x);
				});
				result.AddRange(exportItems);
			} while (page.HasNextPage);

			return result;
		}

		/// <summary>
		/// The method to fetch client ticket data for export
		/// </summary>
		/// <param name="ticketCsvExportParameterModel"></param>
		/// <returns></returns>
		private async Task<IList<TicketClientExportModel>> GetExportDataForClient(TicketCsvExportParameterModel ticketCsvExportParameterModel)
		{
			var result = new List<TicketClientExportModel>();
			var page = PagedList.ForLoop<Ticket>();

			do
			{
				// process data in small chunks, not to consume too much memory
				page = await _ticketRepository.GetPagedExportData(ticketCsvExportParameterModel.DateFrom,ticketCsvExportParameterModel.DateTo, ticketCsvExportParameterModel.BrandId, page.PageIndex + 1);
				var exportItems = page.Items.Select(x => { return _mapper.Map<TicketClientExportModel>(x); });
				result.AddRange(exportItems);
			} while (page.HasNextPage);

			return result;
		}
	}
}