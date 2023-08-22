using System;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The search service update step
	/// </summary>
	public class UpdateTicketSearchService : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The mediator handler
		/// </summary>
		private readonly IMediatorHandler _mediator;
		#endregion

		#region Inputs

		/// <summary>
		/// The ticket identifier
		/// </summary>
		public Guid TicketId { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="mediator"></param>
		public UpdateTicketSearchService(IMediatorHandler mediator) => _mediator = mediator;
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			_mediator.SendCommand(new UpdateTicketIndexCommand() { TicketId = TicketId }).Wait();
			return ExecutionResult.Next();
		}
		#endregion
	}
}
