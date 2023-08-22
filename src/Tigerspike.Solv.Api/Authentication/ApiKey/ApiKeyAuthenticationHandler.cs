using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Api.Authentication.ApiKey
{
	/// <summary>
	/// ApiKeyAuthenticationHandler
	/// </summary>
	public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
	{
		private readonly IApiKeyRepository _apiKeyRepository;
		private readonly IWebHostEnvironment _hostingEnvironment;

		/// <summary>
		/// ApiKeyAuthenticationHandler
		/// </summary>
		public ApiKeyAuthenticationHandler(
			IOptionsMonitor<ApiKeyAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			IApiKeyRepository apiKeyRepository,
			ISystemClock clock,
			IWebHostEnvironment hostingEnvironment) : base(options, logger, encoder, clock)
		{
			_apiKeyRepository = apiKeyRepository;
			_hostingEnvironment = hostingEnvironment;
		}

		/// <summary>
		/// HandleAuthenticateAsync
		/// </summary>
		/// <returns>Authentication ticket on success</returns>
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var apiKey = GetApiKey(Request);

			if (apiKey != null)
			{
				var validationResult = await _apiKeyRepository.IsValidApiKey(apiKey);
				switch (validationResult)
				{
					case true:
						var brandId = await _apiKeyRepository.GetBrandIdFromApiKey(apiKey);
						var authenticationTicket = CreateAuthenticationTicket(brandId.Value);
						return AuthenticateResult.Success(authenticationTicket);

					case false:
						return AuthenticateResult.Fail("Supplied ApiKey is revoked");

					default:
						return AuthenticateResult.Fail("Supplied ApiKey is invalid");
				}
			}
			else
			{
				return AuthenticateResult.NoResult();
			}
		}

		private string GetApiKey(HttpRequest request)
		{
			var apiKeyResolutions = new [] {
				GetApiKeyFromHeaders(request.Headers),
				GetApiKeyFromQueryString(request.QueryString),
			};

			return apiKeyResolutions.FirstOrDefault(x => x != null);
		}

		private string GetApiKeyFromHeaders(IHeaderDictionary headers)
		{
			if (headers.TryGetValue("Authorization", out var authHeaders))
			{
				var authHeader = authHeaders.FirstOrDefault();
				if (authHeader != null && authHeader.StartsWith(ApiKeyAuthentication.Scheme))
				{
					// Match the "ApiKey ..." pattern, and take the key from it.
					var match = Regex.Match(authHeader, $"{ApiKeyAuthentication.Scheme} (.+)");
					return match.Success ? match.Groups[1].Value : null;
				}
			}

			return null;
		}

		private string GetApiKeyFromQueryString(QueryString queryString)
		{
			if (queryString.HasValue)
			{
				// Match the "apikey=" pattern, and take the key from it.
				var match = Regex.Match(queryString.Value, $"{ApiKeyAuthentication.Scheme}=([^&]+)", RegexOptions.IgnoreCase);
				return match.Success ? match.Groups[1].Value : null;
			}

			return null;
		}

		private AuthenticationTicket CreateAuthenticationTicket(Guid brandId)
		{
			var claims = new[]
			{
				new Claim(SolvClaimTypes.BrandId, brandId.ToString()),
				new Claim(ClaimTypes.Role, SolvRoles.Client)
			};

			var identity = new ClaimsIdentity(claims, ApiKeyAuthentication.Scheme);
			var identities = new List<ClaimsIdentity> { identity };
			var principal = new ClaimsPrincipal(identities);

			return new AuthenticationTicket(principal, ApiKeyAuthentication.Scheme);
		}
	}
}