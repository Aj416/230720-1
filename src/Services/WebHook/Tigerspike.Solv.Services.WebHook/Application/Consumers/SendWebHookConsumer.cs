using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Email;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Messaging.WebHook;
using Tigerspike.Solv.Services.WebHook.Configuration;

namespace Tigerspike.Solv.Services.WebHook.Application.Consumers
{

	public class SendWebHookConsumer : IConsumer<ISendWebHookCommand>
	{
		private readonly ILogger<SendWebHookConsumer> _logger;
		private readonly ISignatureService _signatureService;
		private readonly ITemplateService _templateService;
		private readonly WebHookOptions _options;

		public SendWebHookConsumer(
			ILogger<SendWebHookConsumer> logger,
			ISignatureService signatureService,
			ITemplateService webHookTemplateService,
			IOptions<WebHookOptions> options
		)
		{
			_logger = logger;
			_signatureService = signatureService;
			_templateService = webHookTemplateService;
			_options = options?.Value;
		}

		public async Task Consume(ConsumeContext<ISendWebHookCommand> context)
		{
			_logger.LogDebug($"WebHook push initiated for queue item: {context.MessageId} (receiver: {context.Message.Url})");

			string body, url;
			try
			{
				url = _templateService.Render(context.Message.Url, context.Message.Data);
				body = _templateService.Render(context.Message.Body, context.Message.Data);
			}
			catch
			{
				_logger.LogError($"Error while creating body for webhook notification at {context.Message.Url} from template: {context.Message.Body}");
				throw;
			}

			try
			{
				using var httpClient = CreateHttpClient();
				using var payload = new StringContent(body, Encoding.UTF8, context.Message.ContentType);
				await SignPayload(payload, context.Message.Secret);

				var notification = new HttpRequestMessage(new HttpMethod(context.Message.Verb), url)
				{
					Content = payload
				};
				Authorize(notification, context.Message.Authorization);

				var response = await httpClient.SendAsync(notification);
				if (response.IsSuccessStatusCode)
				{
					_logger.LogDebug($"WebHook succesfully pushed to: {url}");
				}
				else
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					_logger.LogWarning($"WebHook pushed to: {url} with payload {body}\nbut target responded with {response.StatusCode} {responseBody}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occured while pushing webhook notification to: {url} with payload: {body}");
				throw;
			}
		}

		private void Authorize(HttpRequestMessage request, string authorizationHeader)
		{
			if (authorizationHeader != null)
			{
				request.Headers.Add("Authorization", authorizationHeader);
			}
		}

		private async Task SignPayload(HttpContent payload, string secret)
		{
			if (secret != null)
			{
				var signature = await GetPayloadSignature(payload, secret);
				payload.Headers.Add("X-Solv-Signature", new[] { signature });
			}
		}

		private async Task<string> GetPayloadSignature(HttpContent payload, string secret)
		{
			var input = await payload.ReadAsStringAsync();
			return "sha1=" + _signatureService.GenerateSha1(input, secret);
		}

		private HttpClient CreateHttpClient()
		{
			var client = new HttpClient() { Timeout = _options.Timeout };
			client.DefaultRequestHeaders.Add("User-Agent", new[] { _options.UserAgent });
			return client;
		}
	}
}