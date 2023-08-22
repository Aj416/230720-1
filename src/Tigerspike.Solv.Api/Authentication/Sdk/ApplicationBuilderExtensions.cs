using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;

namespace Tigerspike.Solv.Api.Authentication.Sdk
{
	/// <summary>
	/// Extension methods for <see cref="IApplicationBuilder"/> to add the SDK JWT authentication/authorization to the pipeline.
	/// </summary>
	public static class ApplicationBuilderExtensions
	{
		/// <summary>
		/// Add sdk token
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddSdkAuthentication(this AuthenticationBuilder builder,
			Action<SdkAuthenticationOptions> options)
		{
			return builder.AddScheme<SdkAuthenticationOptions, SdkAuthenticationHandler>(
				SdkAuthentication.Scheme, options);
		}
	}
}