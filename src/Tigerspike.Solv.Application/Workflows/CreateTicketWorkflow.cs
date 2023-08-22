using System;
using AutoMapper;
using Tigerspike.Solv.Application.Constants;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Application.Workflows.Steps;
using Tigerspike.Solv.Domain.Commands.Chat;
using WorkflowCore.Interface;

namespace Tigerspike.Solv.Application.Workflows
{
	/// <summary>
	/// The ticket creation workflow
	/// </summary>
	public class CreateTicketWorkflow : IWorkflow<CreateTicketWorkflowModel>
	{
		#region Private Properties

		/// <summary>
		/// The auto mapper
		/// </summary>
		private readonly IMapper _mapper;
		#endregion

		#region Public Properties
		/// <summary>
		/// The workflow identifier
		/// </summary>
		public string Id => WorkflowKeys.CreateTicketKey;

		/// <summary>
		/// The workflow version
		/// </summary>
		public int Version => WorkflowKeys.CreateTicketVersion;
		#endregion

		#region Constructor

		/// <summary>
		/// The constructor to initialize properties
		/// </summary>
		/// <param name="mapper"></param>
		public CreateTicketWorkflow(IMapper mapper) => _mapper = mapper;
		#endregion

		#region Build(IWorkflowBuilder<TicketCreationWorkflowModel> builder)

		/// <summary>
		/// The workflow builder
		/// </summary>
		/// <param name="builder"></param>
		public void Build(IWorkflowBuilder<CreateTicketWorkflowModel> builder)
		{
			builder
				.StartWith(context => Console.WriteLine("Begin ticket creation saga"))
				.Saga(saga => saga
					.StartWith<CreateTicketInitiate>()
						.Input(step => step.InitializationModel, data => _mapper.Map<CreateTicketWorkflowInitializationModel>(data))
					.Then<SendTicketSystemMessages>()
						.Input(step => step.MessagesCommand, data => _mapper.Map<SendInitialChatMessageCommand>(data))
					.Then<CreateTicketFinalize>()
						.Input(step => step.TicketId, data => data.TicketId)
					.Then<UpdateTicketSearchService>()
						.Input(step => step.TicketId, data => data.TicketId)
					.Then<NotifyTicketCreation>()
						.Input(step => step.WorkFlowModel, data => data)
					.Then<NotifyTicketStatusChanged>()
						.Input(step => step.NotificationModel, data => _mapper.Map<TicketStatusChangedNotificationModel>(data))
					.Then<NotifyAvailableTicketsChanged>()
						.Input(step => step.NotificationModel, data => _mapper.Map<CreateTicketNotificationModel>(data))
				)
				.Then(context => Console.WriteLine("End ticket creation saga"));
		}
		#endregion
	}
}
