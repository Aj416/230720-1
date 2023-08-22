using System;
using Microsoft.AspNetCore.Authentication;

namespace Tigerspike.Solv.Api.Authentication.ApiKey
{
	/// <summary>
	/// AuthenticationBuilderExtensions
	/// </summary>
	public static class AuthenticationBuilderExtensions
	{
		/// <summary>
		/// AddApiKeySupport
		/// </summary>
		/// <param name="authenticationBuilder"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder authenticationBuilder,
			Action<ApiKeyAuthenticationOptions> options)
		{
			return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
				ApiKeyAuthentication.Scheme, options);
		}
	}
}