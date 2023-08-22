using System.Threading.Tasks;
using FluentEmail.Core.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using ServiceStack;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Messaging.Notification;
using Tigerspike.Solv.Services.Notification.Application.Services;
using Tigerspike.Solv.Services.Notification.Configuration;

namespace Tigerspike.Solv.Services.Notification.Application.Commands
{
	public class SendEmailMessageConsumer : IConsumer<ISendEmailMessageCommand>
	{
		private readonly IEmailService _emailService;
		private readonly ILogger<SendEmailMessageConsumer> _logger;
		private readonly EmailOptions _emailOptions;

		public SendEmailMessageConsumer(
			IEmailService emailService, Microsoft.Extensions.Options.IOptions<EmailOptions> emailOptions,
			ILogger<SendEmailMessageConsumer> logger)
		{
			_emailService = emailService;
			_emailOptions = emailOptions.Value;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ISendEmailMessageCommand> context)
		{
			_logger.LogInformation($"Sending email to {context.Message.MailTo}" );

			var msg = context.Message;
			var mailFrom = msg.ReplyToTicket ? _emailOptions.TicketEmail :
				!string.IsNullOrEmpty(msg.ReplyTo) ? msg.ReplyTo : _emailOptions.DefaultEmail;

			await _emailService.SendAsync(mailFrom, msg.MailTo, StringUtils.HtmlDecode(msg.Subject), msg.Template, msg.Model, msg.Culture, msg.SenderName,
				msg.Attachment != null ? new Attachment
				{
					Filename = msg.Attachment.Filename,
					ContentType = msg.Attachment.ContentType,
					Data = await msg.Attachment.Content.AsStream(),
					IsInline = false
				} : null);
		}
	}
}