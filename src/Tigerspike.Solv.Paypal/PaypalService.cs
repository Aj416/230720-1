using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Refit;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Paypal.API;

namespace Tigerspike.Solv.Paypal
{
	/// <summary>
	/// A service used to contact PayPal api.
	/// </summary>
	public class PaypalService : IPaymentService
	{
		private readonly PaypalOptions _settings;
		private readonly IPaypalApi _paypalClient;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITimestampService _timestampService;
		private readonly ILogger<PaypalService> _logger;

		/// <inheritdoc/>
		public string PaymentReceiverAccountId => _settings.PaymentReceiverAccountId;

		public PaypalService(
			ITimestampService timestampService,
			IOptions<PaypalOptions> options,
			ILogger<PaypalService> logger,
			IRedisClientsManager redisClientsManager)
		{
			_settings = options?.Value ??
				throw new ArgumentNullException(nameof(options));
			_paypalClient = RefitExtensions.For<IPaypalApi>(_settings.ApiUrl);
			_timestampService = timestampService;
			_logger = logger;
			_redisClientsManager = redisClientsManager;
		}

		/// <inheritdoc/>
		public async Task<string> GetPartnerReferralUrl(Guid advocateId)
		{
			// For more information why we need this,
			// check https://developer.paypal.com/docs/commerce-platform/onboarding/upfront/#1-make-a-partner-referrals-api-call
			try
			{
				// Get the access token to call PayPal endpoints.
				var token = await GetAccessToken();
				// Build the request body
				var body = JsonConvert.SerializeObject(GetPartnerReferralApiBody(advocateId.ToString()));
				// Get the referral urls.
				var response = await _paypalClient.GetPartnerReferrals($"Bearer {token.AccessToken}", _settings.BNCode, body);
				// Extract the browser url from the response
				var url = JObject.Parse(response)["links"][1]["href"];

				return url.ToString();
			}
			catch (ApiException ex)
			{
				var msg = "PayPal api call failed: \n{0}";
				_logger.LogError(msg, ex.Content);
				throw new Exception(string.Format(msg, ex.Message));
			}
		}

		/// <inheritdoc/>
		public async Task<(string merchantId, bool paymentReceivable, bool emailVerified)> GetPayPalSolverStatus(Guid advocateId)
		{
			// For more information why we need this,
			// check https://developer.paypal.com/docs/api/partner-referrals/v1/#merchant-integration_status
			try
			{
				var token = await GetAccessToken();

				// Get the status using the advocate id as a tracking id
				var response = await _paypalClient.CheckSolverStatusByTrackingId($"Bearer {token.AccessToken}", _settings.BNCode, _settings.CallerPartnerId, advocateId.ToString());
				var json = JObject.Parse(response);

				// Get the merchant Id of the solver if exists, if not the previous api call will throw 404 exception.
				var merchantId = json["merchant_id"].ToString();

				// Call another endpoint to get the status after we got the merchant id
				response = await _paypalClient.ValidateSolverPermissions($"Bearer {token.AccessToken}", _settings.BNCode, _settings.CallerPartnerId, merchantId);
				json = JObject.Parse(response);

				// Are we authorize to send the solver payments.
				var paymentsReceivableRaw = json["payments_receivable"].ToString();
				// Did the solver verified the PayPal account.
				var emailConfirmedRaw = json["primary_email_confirmed"].ToString();
				// The scopes (permissions) granted to Solv
				var scopes = json.SelectToken("oauth_integrations[0].oauth_third_party[0].scopes");

				// Solv can make payments if we are granted the correct permissions asked before.
				var paymentReceivable = paymentsReceivableRaw.Equals("true", StringComparison.InvariantCultureIgnoreCase) && scopes != null && scopes.Any();

				var emailVerified = emailConfirmedRaw.Equals("true", StringComparison.InvariantCultureIgnoreCase);

				return (merchantId, paymentReceivable, emailVerified);
			}
			catch (ApiException ex)
			{
				// In case the tracking id (advocateId) is not recognized by PayPal, it means the solver hasn't yet visited PayPal.
				if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return (null, false, false);
				}
				var msg = "PayPal api call failed: \n{0}";
				_logger.LogError(msg, ex.Content);
				throw new Exception(string.Format(msg, ex.Message));
			}
		}

		/// <inheritdoc/>
		public async Task<(string url, string baToken)> GenerateBillingAgreement(Guid brandId)
		{
			try
			{
				// Get the access token to call PayPal endpoints.
				var token = await GetAccessToken();

				// Build the request body
				var body = JsonConvert.SerializeObject(GetBillingAgreementTokenRequestBody());
				// Get the referral urls.
				var json = await _paypalClient.GetBillingAgreementToken($"Bearer {token.AccessToken}", _settings.BNCode, body);
				var response = JObject.Parse(json);
				// Extract the browser url from the response
				var url = response["links"][0]["href"].ToString();
				// Extract billing agreement token and save it
				var baToken = response["token_id"].ToString();

				return (url, baToken);

			}
			catch (ApiException ex)
			{
				var msg = "PayPal api call failed: \n{0}";
				_logger.LogError(msg, ex.Content);
				throw new Exception(string.Format(msg, ex.Message));
			}
		}

		/// <inheritdoc/>
		public async Task<(string payerId, string billingAgreementId)> FinalizeBillingAgreement(Guid brandId, string billingAgreementToken)
		{
			try
			{
				// Get PayPal service access token
				var token = await GetAccessToken();

				// Get the referral urls.
				var response = await _paypalClient.FinalizeBillingAgreement($"Bearer {token.AccessToken}", _settings.BNCode, JsonConvert.SerializeObject(new { token_id = billingAgreementToken }));
				var json = JObject.Parse(response);

				// Extract the browser url from the response
				var agreementState = json["state"].ToString();

				// If the state of the agreement is not active, then the payer rejected the agreement
				if (!agreementState.Equals("ACTIVE", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exception("PayPal permissions were not granted for Solv");
				}

				// get the paypal payer id of the task master (client/brand)
				var payerId = json.SelectToken("payer.payer_info.payer_id").ToString();
				// get the billing agreement id.
				var billingAgreementId = json.SelectToken("id").ToString();

				return (payerId, billingAgreementId);

			}
			catch (ApiException ex)
			{
				var msg = "PayPal api call failed: \n{0}";
				_logger.LogError(msg, ex.Content);
				throw new Exception(string.Format(msg, ex.Message));
			}
		}

		/// <inheritdoc/>
		public async Task<string> SetRiskTransactionContext(RiskTransactionContext context)
		{
			// For more information check:
			// https://developer.paypal.com/docs/limited-release/raas/v1/api/#transaction-contexts_set
			try
			{
				// arrange
				var token = await GetAccessToken();
				var riskContextBody = JsonConvert.SerializeObject(GetRiskTransactionContextBody(context));
				var merchantId = context.ReceiverPayPalAccountId;
				var trackingId = context.TrackingId.ToString();

				// make the actual call
				await _paypalClient.SetRiskTransactionContext($"Bearer {token.AccessToken}", _settings.BNCode, merchantId, trackingId, riskContextBody);

				// return tracking id
				return trackingId;
			}
			catch (ApiException ex)
			{
				var msg = "PayPal api call failed: \n{0}";
				_logger.LogError(msg, ex.Content);
				throw new Exception(string.Format(msg, ex.Message));
			}
		}

		/// <inheritdoc/>
		public async Task<string> ExecutePayment(string requestId, string trackingId, string payerId, string billingAgreementId, decimal amount, string currencyCode, string payeeId, string invoiceId, string description, IList<(string name, decimal ticketPrice)> items)
		{
			// For more information why we need this,
			// check https://developer.paypal.com/docs/api/orders/v2/
			try
			{
				// Get the access token to call PayPal endpoints.
				var token = await GetAccessToken();

				// Generate the body of create order (payment) request.
				var createOrderBody = JsonConvert.SerializeObject(GetCreatePaymentBody(payerId, amount, currencyCode, payeeId, invoiceId, description, items));

				// 1- Call the create order (Payment) api and extract the id.
				var rawResponse = await _paypalClient.CreatePayment($"Bearer {token.AccessToken}", _settings.BNCode, createOrderBody);
				var response = JObject.Parse(rawResponse);
				var paymentId = response.SelectToken("id").ToString();

				if (string.IsNullOrEmpty(paymentId))
				{
					throw new Exception($"PayPal payment couldn't be created for { payerId } and { payeeId }");
				}

				// 2- Call the get payment api to make sure the payment is created.
				rawResponse = await _paypalClient.GetPaymentDetail($"Bearer {token.AccessToken}", _settings.BNCode, paymentId);
				response = JObject.Parse(rawResponse);
				var status = response.SelectToken("status").ToString();

				// If the state of the payment is not created then we have a problem.
				if (!status.Equals("CREATED", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exception($"PayPal payment wasn't created for { payerId } and { payeeId }, the status returned is {status}");
				}

				// Create the body of the capture order (payment) api call using the billing agreement acquired from the client (TaskMaster).
				var capturePaymentBody = JsonConvert.SerializeObject(GetCapturePaymentBody(billingAgreementId)).ToString();

				// 3- Execute the payment (order)
				rawResponse = await _paypalClient.ExecutePayment($"Bearer {token.AccessToken}", _settings.BNCode, requestId, trackingId, paymentId, capturePaymentBody);
				response = JObject.Parse(rawResponse);
				status = response.SelectToken("status").ToString();

				// If the state of the payment is not completed then there is a problem on PayPal side.
				if (!status.Equals("COMPLETED", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exception($"PayPal payment { paymentId } couldn't be executed for { payerId } and { payeeId }, the status returned is {status}");
				}

				return paymentId;
			}
			catch (ApiException ex)
			{
				var msg = $"PayPal api call failed: \n{ex.Message}\n{ex.Content}";
				_logger.LogError(msg);
				throw new Exception(msg);
			}
		}

		private async Task<AuthToken> GetAccessToken()
		{
			// Encode the credentials for basic authentication token.
			var credentials = $"{_settings.ClientId}:{_settings.ClientSecret}".ToBase64();

			// Create secure cache key for the given credentials
			var cacheKey = credentials.ToHmacSha256(_settings.ClientSecret);

			// Try to acquire AuthToken from cache if previous is still valid
			using var client = _redisClientsManager.GetClient();
			var token = client.Get<AuthToken>(cacheKey);
			if (token == null)
			{
				// Get the access token by calling PayPal endpoints.
				_logger.LogDebug("Fetching PayPal auth token from authorization service");
				token = await _paypalClient.GetToken($"Basic {credentials}", _settings.BNCode, "grant_type=client_credentials");

				// Store acquired token in cache for future reuse
				var expirationTimestamp = _timestampService.GetUtcTimestamp()
					.AddSeconds(token.ExpiresIn) // make token valid for the time sent by the issuer ...
					.AddMinutes(-3); // ... but maybe some time less, so we can be sure, that it would not expire in the meantime
				client.Set(cacheKey, token, expirationTimestamp);
			}
			else
			{
				_logger.LogDebug("Fetched PayPal auth token from cache");
			}

			return token;
		}

		/// <summary>
		/// A fixed body of the api request to POST /v1/customer/partner-referrals
		/// </summary>
		/// <param name="trackingId">A tracking id we pass to PayPal so we can query the status later</param>
		private object GetPartnerReferralApiBody(string trackingId)
		{
			return new
			{
				customer_data = new
				{
					partner_specific_identifiers = new[]
						{
							new { type = "TRACKING_ID", value = trackingId }
						}
				},
				requested_capabilities = new[]
					{
						new
						{
							capability = "API_INTEGRATION",
								api_integration_preference = new
								{
									partner_id = _settings.CallerPartnerId,
										rest_api_integration = new { integration_method = "PAYPAL", integration_type = "THIRD_PARTY" },
										rest_third_party_details = new
										{
											partner_client_id = _settings.ClientId,
												feature_list = new [] { "PAYMENT", "REFUND", "ADVANCED_TRANSACTIONS_SEARCH", "ACCESS_MERCHANT_INFORMATION" }
										}
								}
						}
					},
				web_experience_preference = new
				{
					partner_logo_url = _settings.PartnerLogo,
					return_url = _settings.SolverReturnUrl,
					// action_renewal_url = "https://example.com/renew-prefill-url"
				},
				collected_consents = new[] { new { type = "SHARE_DATA_CONSENT", granted = true } },
				products = new[] { "EXPRESS_CHECKOUT" }
			};
		}

		/// <summary>
		/// A fixed body of the api request to POST /v1/billing-agreements/agreement-tokens
		/// </summary>
		private object GetBillingAgreementTokenRequestBody()
		{
			return new
			{
				description = "Billing Agreement for future Solv tickets charges",
				payer = new
				{
					payment_method = "PAYPAL"
				},
				plan = new
				{
					type = "CHANNEL_INITIATED_BILLING",
					merchant_preferences = new
					{
						return_url = _settings.ClientReturnUrl,
						cancel_url = _settings.ClientReturnUrl,
						notify_url = _settings.ClientReturnUrl,
						accepted_pymt_type = "ANY",
						skip_shipping_address = true,
						immutable_shipping_address = true
					}
				}
			};
		}

		private object GetCreatePaymentBody(string payerId, decimal amount, string currencyCode, string payeeId, string invoiceId, string description, IList<(string name, decimal amount)> items)
		{
			return new
			{
				intent = "CAPTURE",
				payer = new
				{
					payer_id = payerId
				},
				purchase_units = new[]
					{
						new
						{
							description,
							invoice_id = invoiceId,
							amount = new
							{
								currency_code = currencyCode,
								value = amount.ToString("0.00"),
								breakdown = new
								{
									item_total = new
									{
										currency_code = currencyCode,
										value = amount.ToString("0.00"),
									}
								}
							},
							payee = new
							{
								merchant_id = payeeId
							},
							items = items.Select(item => new
							{
								item.name,
								unit_amount = new
								{
									currency_code = currencyCode,
									value = item.amount.ToString("0.00")
								},
								quantity = "1"
							})
						}
					}
			};
		}

		private object GetCapturePaymentBody(string billingAgreementId)
		{
			return new
			{
				payment_source = new
				{
					token = new
					{
						id = billingAgreementId,
						type = "BILLING_AGREEMENT"
					}
				}
			};
		}

		private object GetRiskTransactionContextBody(RiskTransactionContext context)
		{
			return new
			{
				additional_data = new object[] {
					new {
						key = "sender_account_id",
						value = context.SenderAccountId,
					},
					new {
						key = "sender_create_date",
						value = context.SenderCreatedDate,
					},
					new {
						key = "receiver_account_id",
						value = context.ReceiverAccountId,
					},
					new {
						key = "receiver_create_date",
						value = context.ReceiverCreatedDate,
					},
					new {
						key = "receiver_email",
						value = context.ReceiverEmail,
					},
					new {
						key = "receiver_address_country_code",
						value = context.ReceiverAddressCountryCode ?? string.Empty, // paypal API does not accept "null", so we send an empty string if there was no code
					},
					new {
						key = "recipient_popularity_score",
						value = string.Format("{0:00}", context.ReceiverPopularityScore ?? 0m),
					},
					new {
						key = "first_interaction_date",
						value = context.FirstInteractionDate,
					},
					new {
						key = "txn_count_total",
						value = context.TxnCountTotal.ToString(),
					},
					new {
						key = "transaction_is_tangible",
						value = "0",
					},
				}
			};
		}

	}
}