using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.SignalR;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The ticket status changed notification step
	/// </summary>
	public class NotifyTicketStatusChanged : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The chat hub
		/// </summary>
		private readonly IHubContext<ChatHub> _chatHub;
		#endregion

		#region Inputs

		/// <summary>
		/// The ticket status changed notification model
		/// </summary>
		public TicketStatusChangedNotificationModel NotificationModel { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="chathub"></param>
		public NotifyTicketStatusChanged(IHubContext<ChatHub> chathub) => _chatHub = chathub;
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			var userIds = new Guid?[] { NotificationModel.AdvocateId, NotificationModel.CustomerId }
				.Where(x => x.HasValue)
				.Select(s => s.ToString()).ToList();

			_chatHub.Clients.Users(userIds)
			.SendAsync(ChatHubConstants.TICKET_STATUS_CHANGED, new TicketStatusChangedModel(NotificationModel.TicketId, NotificationModel.ToStatus, NotificationModel.AdvocateId, NotificationModel.AdvocateFirstName, NotificationModel.AdvocateCsat, NotificationModel.ClosedBy)).Wait();

			return ExecutionResult.Next();
		}
		#endregion
	}
}
