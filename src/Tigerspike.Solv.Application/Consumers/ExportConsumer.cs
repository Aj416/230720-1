using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Export;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Storage.Interfaces;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Consumers
{
	public class ExportConsumer :
		IConsumer<CreateTicketsExportCommand>
	{
		private readonly ILogger<ExportConsumer> _logger;
		private readonly ITicketService _ticketService;
		private readonly ITimestampService _timestampService;
		private readonly IUploaderService _uploaderService;
		private readonly ICsvSerializer _csvSerializer;
		private readonly StorageOptions _storageOptions;
		private readonly ISignedUrlGenerator _signedUrlGenerator;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;

		public ExportConsumer(
			ITicketService ticketService,
			IBus bus, Microsoft.Extensions.Options.IOptions<BusOptions> busOptions,
			ITimestampService timestampService,
			IUploaderService uploaderService,
			ICsvSerializer csvSerializer,
			Microsoft.Extensions.Options.IOptions<StorageOptions> storageOptions,
			ISignedUrlGenerator signedUrlGenerator,
			ITemplateService templateService,
			ILogger<ExportConsumer> logger)
		{
			_ticketService = ticketService;
			_bus = bus ?? throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
			_timestampService = timestampService;
			_uploaderService = uploaderService;
			_csvSerializer = csvSerializer;
			_storageOptions = storageOptions.Value;
			_signedUrlGenerator = signedUrlGenerator;
			_logger = logger;
		}


		public async Task Consume(ConsumeContext<CreateTicketsExportCommand> context)
		{
			_logger.LogInformation("Started ticket export generation for {recipientAddress}",
				context.Message.RecipientAddress);

			var exportParameters = new TicketCsvExportParameterModel()
			{
				DateFrom = context.Message.From,
				DateTo = context.Message.To,
				BrandId = context.Message.BrandId,
				TriggeredBy = context.Message.TriggeredBy
			};

			var timestamp = _timestampService.GetUtcTimestamp();

			var filename = context.Message.Scheduled ?
				$"solv-{context.Message.From:yyyyMMddHHmmss}.csv" :
				$"solv-{timestamp:yyyyMMddHHmmss}.csv";

			string reportUrl;
			using (var csvStream = await _ticketService.GetExportData(exportParameters))
			{
				var objectKey = context.Message.Scheduled ? $"{_storageOptions.ExportScheduledPrefix}/{filename}" : filename;
				var bucket = _storageOptions.ExportBucket;
				_logger.LogInformation("Uploading ticket export report", context.Message.RecipientAddress);
				await _uploaderService.Upload(csvStream, objectKey, "text/csv", bucket);
				reportUrl = _signedUrlGenerator.Generate(bucket, objectKey, TimeSpan.FromDays(7));
			}

			if (!context.Message.Scheduled)
			{
				_logger.LogInformation("Sending ticket export report link via email to {recipientAddress}",
					context.Message.RecipientAddress);

				var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

				await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
				{
					MailTo = context.Message.RecipientAddress,
					Subject = "Requested ticket export is ready",
					Template = EmailTemplates.TicketExport.ToString(),
					Model = new Dictionary<string, object>
					{
						{"From", context.Message.From?.ToShortDateString() ?? "-"},
						{"To", context.Message.To?.ToShortDateString() ?? "-"},
						{"ReportUrl", reportUrl},
					}
				});
			}
		}
	}
}
