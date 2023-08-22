using System;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The ticket creation finalization
	/// </summary>
	public class CreateTicketFinalize : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The mediator handler
		/// </summary>
		private readonly IMediatorHandler _mediator;

		/// <summary>
		/// The ticket repository
		/// </summary>
		private readonly ITicketRepository _ticketRepository;

		/// <summary>
		/// The unit of work
		/// </summary>
		private readonly IUnitOfWork _uow;
		#endregion

		#region Inputs

		/// <summary>
		/// The workflow model
		/// </summary>
		public Guid TicketId { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="mediator"></param>
		/// <param name="ticketRepository"></param>
		/// <param name="uow"></param>
		public CreateTicketFinalize(IMediatorHandler mediator, ITicketRepository ticketRepository, IUnitOfWork uow)
		{
			_mediator = mediator;
			_ticketRepository = ticketRepository;
			_uow = uow;
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
			SetTicketToReady().Wait();

			return ExecutionResult.Next();
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// The method to set the ticket to ready state
		/// </summary>
		/// <returns></returns>
		private async Task SetTicketToReady()
		{
			await _ticketRepository.SetTicketReady(TicketId);
			await Commit();
		}

		/// <summary>
		/// The helper method to commit the changes
		/// </summary>
		/// <returns></returns>
		private async Task Commit()
		{
			try
			{
				var commandResponse = await _uow.SaveChangesAsync();
				if (commandResponse > 0)
				{
					return;
				}
			}
			catch (System.Exception ex)
			{
				await _mediator.RaiseEvent(new DomainNotification("CommitError", string.Join(" -> ", ex.WithInnerExceptions().Select(x => x.Message))));
				return;
			}

			await _mediator.RaiseEvent(new DomainNotification("CommitError",
				"We had a problem during saving your data."));
		}
		#endregion
	}
}
