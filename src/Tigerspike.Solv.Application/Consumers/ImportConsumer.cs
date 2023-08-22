using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;

namespace Tigerspike.Solv.Application.Consumers
{
	public class ImportConsumer :
		IConsumer<ImportTicketCommand>
	{
		private readonly ILogger<ImportConsumer> _logger;
		private readonly ITicketService _ticketService;
		private readonly IValidator<ImportTicketCommand> _importTicketCommandValidator;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IDomainNotificationHandler _domainNotificationHandler;
		private readonly ITimestampService _timestampService;
		private readonly IMediator _mediator;

		public ImportConsumer(
			ITicketService ticketService,
			IValidator<ImportTicketCommand> importTicketCommandValidator,
			IServiceScopeFactory scopeFactory,
			IDomainNotificationHandler domainNotificationHandler,
			ITimestampService timestampService,
			IMediator mediator,
			ILogger<ImportConsumer> logger)
		{
			_ticketService = ticketService;
			_importTicketCommandValidator = importTicketCommandValidator;
			_scopeFactory = scopeFactory;
			_domainNotificationHandler = domainNotificationHandler;
			_timestampService = timestampService;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ImportTicketCommand> context)
		{
			_logger.LogDebug("Started ticket import [{referenceId}]", context.Message.ReferenceId);

			// validate the input
			var validationResult = await _importTicketCommandValidator.ValidateAsync(context.Message);
			if (validationResult.IsValid)
			{
				_logger.LogDebug("Validation OK on ticket import [{referenceId}]", context.Message.ReferenceId);

				try
				{
					// try to import the ticket
					var ticketId = await _mediator.Send(context.Message);
					if (ticketId != null)
					{
						_logger.LogDebug("Ticket import [{referenceId}] successfully [{ticketId}]", context.Message.ReferenceId, ticketId);
					}
					else
					{
						var failureReason = string.Join(",", _domainNotificationHandler.GetNotifications().Select(x => x.Value));
						await AddImportFailure(context.Message, failureReason);
					}
				}
				catch (Exception ex)
				{
					var failureReason = string.Join(",", ex.WithInnerExceptions().Select(x => x.Message));
					await AddImportFailure(context.Message, failureReason);
				}
			}
			else
			{
				var failureReason = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage));
				await AddImportFailure(context.Message, failureReason);
			}

		}

		private async Task AddImportFailure(ImportTicketCommand cmd, string failureReason)
		{
			// since our transaction is now most-likely corrupted, and we use scoped unit of work, we have to create fresh scope to report failure handlers
			using (var scope = _scopeFactory.CreateScope())
			{
				var freshMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
				await freshMediator.Send(new AddTicketImportFailureCommand(cmd.ImportId, cmd.RawInput, failureReason));
			}

		}

	}
}
