using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Api.Authentication.Sdk
{
    /// <summary>
    /// SdkAuthenticationHandler
    /// </summary>
    public sealed class SdkAuthenticationHandler : AuthenticationHandler<SdkAuthenticationOptions>
    {
        private const string ApplicationIdKey = "applicationId";
        private const string TokenStart = "Bearer";
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<SdkAuthenticationHandler> _logger;

        /// <summary>
        /// SdkAuthenticationHandler
        /// </summary>
        public SdkAuthenticationHandler(
            IApiKeyRepository apiKeyRepository,
            IOptions<JwtOptions> jwtOptions,
            IOptionsMonitor<SdkAuthenticationOptions> optionsMonitor,
            ILoggerFactory loggerFactory,
            ILogger<SdkAuthenticationHandler> logger,
            UrlEncoder urlEncoder,
            ISystemClock clock)
            : base(optionsMonitor, loggerFactory, urlEncoder, clock)
        {
            _apiKeyRepository = apiKeyRepository;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// HandleAuthenticateAsync
        /// </summary>
        /// <returns>Authentication ticket on success</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string header = Context.Request.Headers[HeaderNames.Authorization];

            return await GetAuthenticationResult(header);
        }

        private async Task<AuthenticateResult> GetAuthenticationResult(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                Logger.LogInformation($"Header {nameof(HeaderNames.Authorization)} is empty, returning none");
                return AuthenticateResult.NoResult();
            }

            var token = header.Substring(TokenStart.Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                Logger.LogInformation(
                    $"Token in header {nameof(HeaderNames.Authorization)} is empty, returning none");
                return AuthenticateResult.NoResult();
            }

            try
            {
                IDictionary<string, string> dic;

                try
                {
                    dic = new JwtBuilder()
                        .WithAlgorithm(new HMACSHA256Algorithm())
                        .WithUrlEncoder(new JwtBase64UrlEncoder())
                        .WithSerializer(new JsonNetSerializer())
                        .WithSecret(_jwtOptions.SdkSecretKey)
                        .MustVerifySignature()
                        .Decode<Dictionary<string, string>>(token);
                }
                catch (TokenExpiredException)
                {
                    return AuthenticateResult.Fail("Token expired.");
                }
                catch (SignatureVerificationException)
                {
                    return AuthenticateResult.Fail("Signature verification failed.");
                }
                catch (InvalidTokenPartsException)
                {
                    _logger.LogError("Wrong token format for sdk authentication with {token}", token);
                    return AuthenticateResult.Fail("Wrong token format");
                }

                // Verify the application id

                var applicationId = !dic.ContainsKey(ApplicationIdKey) ? null : dic[ApplicationIdKey];

                if (applicationId == null)
                {
                    Logger.LogInformation("Application id is missing");

                    return AuthenticateResult.Fail("Wrong application id.");
                }

                var validationResult = await _apiKeyRepository.IsValidApplicationId(applicationId);

                Guid? brandId;

                switch (validationResult)
                {
                    case true:
                        brandId = await _apiKeyRepository.GetBrandIdFromApplicationId(applicationId);
                        break;
                    case false:
                        return AuthenticateResult.Fail("Supplied ApiKey is revoked");

                    default:
                        return AuthenticateResult.Fail("Supplied ApiKey is invalid");
                }

                var identity = CreateIdentity(dic, brandId.Value);
                var ticket = CreateTicket(identity, Scheme);

                Logger.LogInformation("Successfully decoded JWT, returning success");
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error decoding JWT: {ex.Message}, returning failure");
                return AuthenticateResult.Fail(ex);
            }
        }

        private static AuthenticationTicket CreateTicket(IIdentity identity, AuthenticationScheme scheme) =>
            new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                scheme.Name);

        private static IIdentity CreateIdentity(IDictionary<string, string> dic, Guid brandId)
        {
            var claims = dic.Select(p => new Claim(p.Key, p.Value)).ToList();

            claims.Add(new Claim(SolvClaimTypes.BrandId, brandId.ToString()));

            return new ClaimsIdentity(claims, SdkAuthentication.Scheme);
        }
    }
}