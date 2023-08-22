using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Events.Ticket;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// A helper method to notify ticket creation completion
	/// </summary>
	public class NotifyTicketCreation : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The mediator handler
		/// </summary>
		private readonly IMediatorHandler _mediator;
		#endregion

		#region Inputs

		/// <summary>
		/// The workflow input
		/// </summary>
		public CreateTicketWorkflowModel WorkFlowModel { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="mediator"></param>
		public NotifyTicketCreation(IMediatorHandler mediator) => _mediator = mediator;
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			_mediator.RaiseEvent(new TicketCreatedEvent(WorkFlowModel.TicketId, WorkFlowModel.ReferenceId, WorkFlowModel.ThreadId,
						WorkFlowModel.BrandId,
						WorkFlowModel.CustomerId, WorkFlowModel.Question,
						WorkFlowModel.SourceId, WorkFlowModel.SourceName, WorkFlowModel.TransportType, WorkFlowModel.IsPractice, WorkFlowModel.Level, WorkFlowModel.Culture)).Wait();

			return ExecutionResult.Next();
		}
		#endregion
	}
}
