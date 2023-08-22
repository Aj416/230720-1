using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.SignalR;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The advocate notification step to update statistics
	/// </summary>
	public class NotifyAdvocateStatisticsUpdated : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The ticket hub
		/// </summary>
		private readonly IHubContext<TicketHub> _ticketHub;
		#endregion

		#region Inputs

		/// <summary>
		/// The advocate id
		/// </summary>
		public string AdvocateId { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="ticketHub"></param>
		public NotifyAdvocateStatisticsUpdated(IHubContext<TicketHub> ticketHub) => _ticketHub = ticketHub;
		#endregion

		#region Run(IStepExecutionContext context)
		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			_ticketHub.Clients.User(AdvocateId).SendAsync(TicketHubConstants.ADVOCATE_STATISTICS_UPDATED).Wait();
			return ExecutionResult.Next();
		}
		#endregion
	}
}
