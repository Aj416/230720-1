using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Services.Notification.Configuration;
using Tigerspike.Solv.Services.Notification.Smooch;

namespace Tigerspike.Solv.Services.Notification.Application.Services
{
	public class MessengerService : IMessengerService
	{
		private readonly MessengerOptions _options;
		private readonly ILogger<MessengerService> _logger;
		private readonly ISmooshApi _client;

		public MessengerService(Microsoft.Extensions.Options.IOptions<MessengerOptions> options,
			ILogger<MessengerService> logger)
		{
			_options = options?.Value;
			_logger = logger;
			_client = RestService.For<ISmooshApi>(_options?.BaseUrl);
		}

		/// <inheritdoc />
		public Task PostMessage(string conversationId, string text)
		{
			var token = GetJwtToken(_options.AppId, _options.KeyId, _options.Secret);
			return _client.PostMessage(token, _options.AppId, conversationId,
				new PostMessageRequest(new PostMessageAuthor(), new PostMessageContent(text)));
		}

		private string GetJwtToken(string appId, string keyId, string secret)
		{
			return "Bearer " + new JwtBuilder()
				.WithAlgorithm(new HMACSHA256Algorithm())
				.WithSecret(secret)
				.AddHeader(HeaderName.KeyId, keyId)
				.AddClaim("scope", "app")
				.Encode();
		}
	}
}