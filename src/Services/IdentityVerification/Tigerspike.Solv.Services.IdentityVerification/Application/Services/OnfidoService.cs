using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Refit;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Messaging.IdentityVerification;
using Tigerspike.Solv.Services.IdentityVerification.Configuration;
using Tigerspike.Solv.Services.IdentityVerification.Domain;
using Tigerspike.Solv.Services.IdentityVerification.Refit;
using Tigerspike.Solv.Services.IdentityVerification.Refit.CreateApplicant;
using Tigerspike.Solv.Services.IdentityVerification.Refit.CreateCheck;
using Tigerspike.Solv.Services.IdentityVerification.Refit.GenerateSdkToken;
using Tigerspike.Solv.Services.IdentityVerification.Refit.GetCheck;
using Tigerspike.Solv.Services.IdentityVerification.Refit.UpdateApplicant;
using Tigerspike.Solv.Services.IdentityVerification.Refit.Webhook;

namespace Tigerspike.Solv.Services.IdentityVerification.Application.Services
{
	/// <summary>
	/// A service used to contact Onfido api.
	/// </summary>
	public class OnfidoService : IIdentityVerificationService
	{
		private readonly OnfidoOptions _options;
		private readonly IOnfidoApi _api;
		private readonly ISignatureService _signatureService;
		private readonly ILogger<OnfidoService> _logger;
		private readonly IMediatorHandler _mediatorHandler;
		private readonly JsonSerializerSettings _serializerSettings;
		private readonly IBus _bus;
		private readonly IPocoDynamo _db;
		private string Token => $"Token token={_options.ApiToken}";

		public OnfidoService(
			IPocoDynamo db,
			ISignatureService signatureService,
			IBus bus,
			IOptions<OnfidoOptions> options,
			ILogger<OnfidoService> logger,
			IMediatorHandler mediatorHandler
			)
		{
			_db = db;
			_options = options?.Value;
			_signatureService = signatureService;
			_bus = bus;
			_logger = logger;
			_mediatorHandler = mediatorHandler;

			_serializerSettings = new JsonSerializerSettings()
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				},
				Converters = new[] {
						new StringEnumConverter(new SnakeCaseNamingStrategy())
					}
			};

			_api = RefitExtensions.For<IOnfidoApi>(_options?.ApiUrl, _serializerSettings);
		}

		/// <inheritdoc />
		public async Task<string> CreateApplicant(string firstName, string lastName)
		{
			try
			{
				var input = new CreateApplicantRequest
				{
					FirstName = firstName,
					LastName = lastName
				};

				var result = await _api.CreateApplicant(Token, input);
				return result.Id;
			}
			catch (ApiException ex)
			{
				var msg = "Onfido api call failed: \n{0}";
				_logger.LogError(msg, ex.Content);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task<string> UpdateApplicant(string applicantId, string firstName, string lastName)
		{
			try
			{
				var result = await _api.UpdateApplicant(Token, applicantId, new UpdateApplicantRequest(firstName, lastName));
				return result.Id;
			}
			catch (ApiException ex)
			{
				if (ex.StatusCode == System.Net.HttpStatusCode.Gone)
				{
					// if applicant data were removed, then too bad
					return null;
				}
				else
				{

					_logger.LogError("Onfido api call failed: \n{0}", ex.Content);
					throw;
				}
			}
		}

		/// <inheritdoc />
		public async Task<string> GenerateSdkToken(string applicantId)
		{
			try
			{
				var input = new GenerateSdkTokenRequest
				{
					ApplicantId = applicantId,
					Referrer = _options.Referrer,
				};

				var result = await _api.GenerateSdkToken(Token, input);
				return result.Token;
			}
			catch (ApiException ex)
			{
				_logger.LogError("Onfido api call failed: \n{0}", ex.Content);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task<(string checkId, string reportUrl)> CreateCheck(string applicantId)
		{
			try
			{
				var input = new CreateCheckRequest
				{
					ApplicantId = applicantId,
					ReportNames = _options.Reports,
				};

				var result = await _api.CreateCheck(Token, input);
				return (result.Id, result.ResultsUri);
			}
			catch (ApiException ex)
			{
				_logger.LogError("Onfido api call failed: \n{0}", ex.Content);
				throw;
			}
		}

		public async Task<bool> ConsumeWebhook(HttpRequest request)
		{
			var payload = await GetPayload(request);
			var signature = GetSignature(request);
			_logger.LogDebug("Consuming Onfido webhook: {payload}", payload);

			if (IsWebhookValid(payload, signature))
			{
				var webhook = GetWebhookModel(payload);
				if (webhook?.Payload?.Object != null)
				{
					return await ConsumeWebhook(webhook.Payload);
				}
				else
				{
					_logger.LogError("Failed to properly deserialize Onfido webhook @{payload}", payload);
					return false;
				}
			}
			else
			{
				_logger.LogError("Onfido webhook signature verification failed for @{payload}", payload);
				return false;
			}
		}

		private async Task<bool> ConsumeWebhook(IdentityWebhookPayload payload)
		{
			switch (payload.ResourceType)
			{
				case ResourceType.Check:
					return await ConsumeCheckWebhook(payload.Object);

				default:
					return true; // just ignore the webhook
			}
		}

		private async Task<GetCheckResponse> GetCheck(string checkId)
		{
			try
			{
				return await _api.GetCheck(Token, checkId);
			}
			catch (ApiException ex)
			{
				_logger.LogError("Onfido api call failed: \n{0}", ex.Content);
				_logger.LogDebug($"Call parameters: token: [{Token}], checkId: [{checkId}]");
				throw;
			}
		}
		private bool IsWebhookValid(string payload, string signature)
		{
			var actualSignature = _signatureService.GenerateSha256(payload, _options.WebhookToken);
			return signature == actualSignature;
		}

		private async Task<bool> ConsumeCheckWebhook(IdentityWebhookObject obj)
		{
			var checkId = obj.Id;
			var item = _db.GetItem<IdentityCheck>(checkId);

			if (item != null && !item.Success.HasValue)
			{
				var checkResponse = await GetCheck(checkId);
				var checkResult = GetCheckResult(checkResponse);
				if (checkResult.HasValue)
				{
					// Update the status
					item.SetValue(checkResult.Value);
					await _db.PutItemAsync(item);

					// Publish integration event
					await _bus.Publish<IIdentityCheckCompletedEvent>(new
					{
						AdvocateId = item.AdvocateId,
						ApplicantId = item.ApplicantId,
						CheckId = checkId,
						Success = checkResult.Value,
						Timestamp = DateTime.UtcNow
					});
				}
			}
			else
			{
				_logger.LogDebug("Check request was not found or already processed for the id: {checkId}", checkId);
			}

			return true;
		}

		private static bool? GetCheckResult(GetCheckResponse response)
		{
			switch (response.Status)
			{
				case CheckStatus.Complete:
					return response.Result == CheckResult.Clear;
				case CheckStatus.Withdrawn:
					return false;
				default:
					return null;
			}
		}

		private IdentityWebhookRequest GetWebhookModel(string payload) =>
			JsonConvert.DeserializeObject<IdentityWebhookRequest>(payload, _serializerSettings);

		private static async Task<string> GetPayload(HttpRequest request)
		{
			using var reader = new StreamReader(request.Body);
			return await reader.ReadToEndAsync();
		}

		private static string GetSignature(HttpRequest request) =>
			request.Headers.TryGetValue("x-sha2-signature", out var signatureHeaders)
				? signatureHeaders.FirstOrDefault()
				: null;
	}
}