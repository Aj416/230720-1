using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using static Tigerspike.Solv.Application.SignalR.TicketHubConstants;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Tigerspike.Solv.Application.Models.Ticket;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The advocate notification step to update available tickets
	/// </summary>
	public class NotifyAvailableTicketsChanged : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The cached brand repository
		/// </summary>
		private readonly ICachedBrandRepository _cachedBrandRepository;

		/// <summary>
		/// The ticket hub
		/// </summary>
		private readonly IHubContext<TicketHub> _ticketHub;
		#endregion

		#region Inputs

		/// <summary>
		/// The notification model
		/// </summary>
		public CreateTicketNotificationModel NotificationModel { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="cachedBrandRepository"></param>
		/// <param name="ticketHub"></param>
		public NotifyAvailableTicketsChanged(ICachedBrandRepository cachedBrandRepository, IHubContext<TicketHub> ticketHub)
		{
			_cachedBrandRepository = cachedBrandRepository;
			_ticketHub = ticketHub;
		}
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			_cachedBrandRepository.AddTicket(NotificationModel.TicketId, NotificationModel.BrandId, NotificationModel.Level);
			_ticketHub.Clients.Group(GetBrandGroupName(NotificationModel.BrandId)).SendAsync(BRAND_AVAILABLE_TICKETS_UPDATED).Wait();

			return ExecutionResult.Next();
		}
		#endregion
	}
}
