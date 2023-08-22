using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Messaging.IdentityVerification;
using Tigerspike.Solv.Services.IdentityVerification.Application.Services;
using Tigerspike.Solv.Services.IdentityVerification.Domain;

namespace Tigerspike.Solv.Services.IdentityVerification.Application.Consumers
{
	public class CreateIdentityCheckCommandConsumer : IConsumer<ICreateIdentityCheckCommand>
	{
		private readonly IIdentityVerificationService _identityVerificationService;
		private readonly ILogger<CreateIdentityCheckCommandConsumer> _logger;
		private readonly IPocoDynamo _db;

		public CreateIdentityCheckCommandConsumer(
			IIdentityVerificationService identityVerificationService,
			IPocoDynamo db,
			ILogger<CreateIdentityCheckCommandConsumer> logger)
		{
			_identityVerificationService = identityVerificationService;
			_db = db;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ICreateIdentityCheckCommand> context)
		{
			var message = context.Message;
			var applicantId = message.ApplicantId;

			if (string.IsNullOrEmpty(applicantId))
			{
				applicantId = await _identityVerificationService.CreateApplicant(message.FirstName, message.LastName);

				// publish integration event
				await context.Publish<IIdentityCreatedEvent>(new
				{
					AdvocateId = message.AdvocateId,
					ApplicantId = applicantId,
					Timestamp = DateTime.UtcNow
				});
			}
			else
			{
				await _identityVerificationService.UpdateApplicant(applicantId, message.FirstName, message.LastName);
			}

			if (applicantId == null)
			{
				// The account is gone, log an error
				_logger.LogError("The applicant id for advocate {0} is still null and the account is probably gone",
					message.AdvocateId);
				return;
			}

			// Create the check in Onfido
			var (checkId, reportUrl) = await _identityVerificationService.CreateCheck(applicantId);

			// Store the check in the database
			await _db.PutItemAsync(new IdentityCheck(checkId, message.AdvocateId, applicantId, reportUrl));

			// publish integration event
			await context.Publish<IIdentityCheckCreatedEvent>(new
			{
				AdvocateId = message.AdvocateId,
				ApplicantId = applicantId,
				CheckId = checkId,
				CheckReportUrl = reportUrl,
				Timestamp = DateTime.UtcNow
			});
		}
	}
}