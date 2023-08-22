using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Localization;
using Tigerspike.Solv.Services.Notification.Configuration;

namespace Tigerspike.Solv.Services.Notification.Application.Services
{
	public class EmailService : IEmailService
	{
		private readonly ITemplateService _templateService;
		private readonly ILocalizationProviderFactory _localizationProviderFactory;
		private readonly IFluentEmailFactory _emailFactory;
		private readonly EmailOptions _emailOptions;
		private readonly ILogger<EmailService> _logger;

		public EmailService(
			ITemplateService templateService,
			ILocalizationProviderFactory localizationProviderFactory,
			IFluentEmailFactory emailFactory,
			IOptions<EmailOptions> emailOptions,
			ILogger<EmailService> logger)
		{
			_templateService = templateService;
			_localizationProviderFactory = localizationProviderFactory;
			_emailFactory = emailFactory;
			_emailOptions = emailOptions.Value;
			_logger = logger;
		}

		/// <inheritdoc/>
		public async Task<bool> SendAsync(string replyTo, string mailTo, string subject, string template, Dictionary<string, object> model, string culture,
			string senderName = null, Attachment attachment = null)
		{
			var maxRetryAttempts = 10;
			var pauseBetweenFailures = TimeSpan.FromSeconds(2);
			var isSuccessful = false;
			var retryPolicy = Policy
				.Handle<Exception>()
				.WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);
			var templatePath =
				$"{AppDomain.CurrentDomain.BaseDirectory}Templates/Email/{template}.template";

			if (!File.Exists(templatePath))
			{
				throw new ArgumentException("template file not found.", nameof(template));
			}

			var emailTemplate = ParseEmailTemplate(templatePath, model, culture);
			subject = subject.Replace("\r", "").Replace("\n", "");

			var email = _emailFactory.Create()
				.SetFrom(replyTo, senderName ?? _emailOptions.DisplayName)
				.ReplyTo(replyTo)
				.To(mailTo)
				.Subject(subject)
				.Body(emailTemplate, true);

			if (attachment != null)
			{
				email = email.Attach(attachment);
			}

			await retryPolicy.ExecuteAsync(async () =>
			{
				var response = await email.SendAsync();
				isSuccessful = response.Successful;
			});
			if (isSuccessful)
			{
				return true;
			}
			else
			{
				_logger.LogError("Error sending email");
				return false;
			}
		}

		private string ParseEmailTemplate(string templateFilePath, Dictionary<string, object> model, string culture)
		{
			var provider = _localizationProviderFactory.GetLocalizationProvider(culture);
			var template = File.ReadAllText(templateFilePath);
			var inter = _templateService.Render(template, model, new { provider.Resources });
			return _templateService.Render(inter, model); // parse the intermediate results once again - in case there were some variables in the localization resources
		}
	}
}