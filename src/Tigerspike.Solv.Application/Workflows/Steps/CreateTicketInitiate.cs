using System.Collections.Generic;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Domain.Enums;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The intial ticket create step
	/// </summary>
	public class CreateTicketInitiate : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The ticket auto response service
		/// </summary>
		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		#endregion

		#region Inputs

		/// <summary>
		/// The initialization model
		/// </summary>
		public CreateTicketWorkflowInitializationModel InitializationModel { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="ticketAutoResponseService"></param>
		public CreateTicketInitiate(ITicketAutoResponseService ticketAutoResponseService) => _ticketAutoResponseService = ticketAutoResponseService;
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			_ticketAutoResponseService.SendResponses(InitializationModel.BrandId, InitializationModel.TicketId, new[] { BrandAdvocateResponseType.TicketCreated }).Wait();

			if (InitializationModel.IsPractice == false)
			{
				_ticketAutoResponseService.SendResponses(InitializationModel.BrandId, InitializationModel.TicketId, new[] { BrandAdvocateResponseType.TicketCutOff }).Wait();
			}

			if (InitializationModel.Level == TicketLevel.Regular)
			{
				_ticketAutoResponseService.SendResponses(InitializationModel.BrandId, InitializationModel.TicketId, new[] { BrandAdvocateResponseType.TicketOpenTimeoutEscalation }).Wait();
			}

			return ExecutionResult.Next();
		}
		#endregion
	}
}
