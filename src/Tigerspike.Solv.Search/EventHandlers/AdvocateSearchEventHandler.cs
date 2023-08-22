using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.User;
using Tigerspike.Solv.Search.Interfaces;

namespace Tigerspike.Solv.Search.EventHandlers
{
	public class AdvocateSearchEventHandler :
		INotificationHandler<AdvocateCreatedEvent>,
		INotificationHandler<AdvocateCsatUpdatedEvent>,
		INotificationHandler<AdvocatePractiseFinishedEvent>,
		INotificationHandler<AdvocateContractAgreedEvent>,
		INotificationHandler<AdvocateInductionCompletedEvent>,
		INotificationHandler<InductionItemViewedEvent>,
		INotificationHandler<AdvocateBrandsAssignedEvent>,
		INotificationHandler<AdvocateBrandEnabledEvent>,
		INotificationHandler<AdvocateBrandDisabledEvent>,
		INotificationHandler<AdvocateQuizAttemptedEvent>,
		INotificationHandler<NewAdvocateBrandsAssignedEvent>,
		INotificationHandler<AdvocatePaymentAccountUpdatedEvent>,
		INotificationHandler<AdvocateVideoWatchedEvent>,
		INotificationHandler<AdvocateIdentityVerificationStatusUpdatedEvent>,
		INotificationHandler<UserBlockedEvent>,
		INotificationHandler<UserUnblockedEvent>,
		INotificationHandler<AdvocateNameChangedEvent>,
		INotificationHandler<AdvocateBrandsRemovedEvent>

	{
		private readonly ISearchService<AdvocateSearchModel> _searchService;
		private readonly IAdvocateService _advocateService;
		private readonly ILogger<AdvocateSearchEventHandler> _logger;

		public AdvocateSearchEventHandler(
			ISearchService<AdvocateSearchModel> searchService,
			IAdvocateService advocateService,
			ILogger<AdvocateSearchEventHandler> logger)
		{
			_searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
			_advocateService = advocateService ?? throw new ArgumentNullException(nameof(advocateService));
			_logger = logger;
		}


		public Task Handle(AdvocateCreatedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateCsatUpdatedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocatePractiseFinishedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocatePaymentAccountUpdatedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateVideoWatchedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateIdentityVerificationStatusUpdatedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(UserUnblockedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.UserId);
		public Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.UserId);
		public Task Handle(AdvocateContractAgreedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateInductionCompletedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(InductionItemViewedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateBrandsAssignedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateBrandEnabledEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateBrandDisabledEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(NewAdvocateBrandsAssignedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateQuizAttemptedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateNameChangedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);
		public Task Handle(AdvocateBrandsRemovedEvent notification, CancellationToken cancellationToken) => GetAndIndexAdvocate(notification.AdvocateId);

		/// <summary>
		/// Fetch the advocate from db and index it.
		/// </summary>
		private async Task GetAndIndexAdvocate(Guid advocateId)
		{
			try
			{
				var model = await _advocateService.GetAdvocateForSearch(advocateId);
				if (model != null)
				{
					await _searchService.Index(model);
				}
			}
			catch(Exception ex)
			{	
				_logger.LogError(ex, "Search Index update failed on AdvocateSearchEventHandler for advocate {0}", advocateId);
				throw new Exception("Error on RedisUpdate", ex);
			}
			
		}
	}
}
