using System;
using AutoMapper;
using Tigerspike.Solv.Application.Constants;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Application.Workflows.Steps;
using WorkflowCore.Interface;

namespace Tigerspike.Solv.Application.Workflows
{
	/// <summary>
	/// The ticket completion workflow
	/// </summary>
	public class CompleteTicketWorkflow : IWorkflow<CompleteTicketWorkflowModel>
	{
		#region Private Properties

		/// <summary>
		/// Auto mapper
		/// </summary>
		private readonly IMapper _mapper;
		#endregion

		#region Public Properties

		/// <summary>
		/// The workflow identifier
		/// </summary>
		public string Id => WorkflowKeys.CompleteTicketKey;

		/// <summary>
		/// The workflow version
		/// </summary>
		public int Version => WorkflowKeys.CompleteTicketVersion;
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properites
		/// </summary>
		/// <param name="mapper"></param>
		public CompleteTicketWorkflow(IMapper mapper) => _mapper = mapper;
		#endregion

		#region Build(IWorkflowBuilder<CompleteTicketWorkflowModel> builder)

		/// <summary>
		/// The workflow builder
		/// </summary>
		/// <param name="builder"></param>
		public void Build(IWorkflowBuilder<CompleteTicketWorkflowModel> builder)
		{
			builder
				.StartWith(context => Console.WriteLine("Begin ticket completion saga"))
				.Saga(saga => saga
					.StartWith<UpdateTicketSearchService>()
						.Input(step => step.TicketId, data => data.TicketId)
					.Then<UpdateAdvocateStatistics>()
						.Input(step => step.AdvocateId, data => data.AdvocateId ?? Guid.Empty)
					.Then<NotifyTicketStatusChanged>()
						.Input(step => step.NotificationModel, data => _mapper.Map<TicketStatusChangedNotificationModel>(data))
					.Then<NotifyAdvocateStatisticsUpdated>()
						.Input(step => step.AdvocateId, data => data.AdvocateId.ToString())
				)
				.Then(context => Console.WriteLine("End ticket completion saga"));
		}
		#endregion
	}
}