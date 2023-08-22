using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Tigerspike.Solv.Application.Workflows.Steps
{
	/// <summary>
	/// The advocate statistics update step
	/// </summary>
	public class UpdateAdvocateStatistics : StepBody
	{
		#region Private Properties

		/// <summary>
		/// The cached ticket repository
		/// </summary>
		private readonly ICachedTicketRepository _cachedTicketRepository;

		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<UpdateAdvocateStatistics> _logger;
		#endregion

		#region Inputs

		/// <summary>
		/// The advocate identifier
		/// </summary>
		public Guid AdvocateId { get; set; }
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="cachedTicketRepository"></param>
		public UpdateAdvocateStatistics(ILogger<UpdateAdvocateStatistics> logger, ICachedTicketRepository cachedTicketRepository) => (_logger, _cachedTicketRepository) = (logger, cachedTicketRepository);
		#endregion

		#region Run(IStepExecutionContext context)

		/// <summary>
		/// The workflow step logic
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override ExecutionResult Run(IStepExecutionContext context)
		{
			var result = _cachedTicketRepository.UpdateStatistics(AdvocateId).Result;
			_logger.LogInformation("CompleteTicket Saga Advocate {0} statistics updated {1}", AdvocateId, JsonConvert.SerializeObject(result));
			return ExecutionResult.Next();
		}
		#endregion
	}
}