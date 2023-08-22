using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Search.Interfaces;

namespace Tigerspike.Solv.Search.EventHandlers
{
	public class AdvocateApplicationSearchEventHandler :
		INotificationHandler<AdvocateApplicationCreatedEvent>,
		INotificationHandler<AdvocateApplicationCompletedEvent>,
		INotificationHandler<AdvocateApplicationInvitedEvent>,
		INotificationHandler<AdvocateApplicationDeletedEvent>,
		INotificationHandler<AdvocateApplicationResponseEmailSentEvent>,
		INotificationHandler<AdvocateApplicationCompletedEmailSentEvent>,
		INotificationHandler<AdvocateApplicationDeclinedEvent>,
		INotificationHandler<AdvocateCreatedEvent>,
		INotificationHandler<AdvocateApplicationNameChangedEvent>
	{
		private readonly IMapper _mapper;
		private readonly ISearchService<AdvocateApplicationSearchModel> _searchService;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly ILogger<AdvocateApplicationSearchEventHandler> _logger;

		public AdvocateApplicationSearchEventHandler(
			IMapper mapper,
			ISearchService<AdvocateApplicationSearchModel> searchService,
			IAdvocateApplicationRepository advocateApplicationRepository,
			ILogger<AdvocateApplicationSearchEventHandler> logger)
		{
			_mapper = mapper;
			_searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
			_advocateApplicationRepository = advocateApplicationRepository;
			_logger = logger;
		}

		public Task Handle(AdvocateApplicationCreatedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public Task Handle(AdvocateApplicationCompletedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public Task Handle(AdvocateApplicationInvitedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public async Task Handle(AdvocateApplicationDeletedEvent notification, CancellationToken cancellationToken) => await _searchService.Delete(notification.ApplicationId);

		public Task Handle(AdvocateApplicationResponseEmailSentEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public Task Handle(AdvocateApplicationCompletedEmailSentEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public Task Handle(AdvocateApplicationDeclinedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public Task Handle(AdvocateApplicationNameChangedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.ApplicationId);

		public Task Handle(AdvocateCreatedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocateApplication(notification.AdvocateId);

		private async Task GetAndIndexAdvocateApplication(Guid applicationId)
		{
			try
			{
				var model = await _advocateApplicationRepository.GetFirstOrDefaultAsync<AdvocateApplicationSearchModel>(_mapper, x => x.Id == applicationId);
				if (model != null)
				{
					await _searchService.Index(model);
				}
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Search Index update failed on AdvocateApplicationSearchEventHandler for advocate application {0}", applicationId);
				throw new Exception("Error on RedisUpdate", ex);
			}
		}
	}
}