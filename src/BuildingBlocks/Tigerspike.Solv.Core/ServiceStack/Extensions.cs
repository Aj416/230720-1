using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using Tigerspike.Solv.Core.Configuration;

namespace Tigerspike.Solv.Core.ServiceStack
{
	public static class ServiceStackExtensions
	{

		/// <summary>
		/// Applies the service stack license if found.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseServiceStack(this IApplicationBuilder builder)
		{
			var options = builder.ApplicationServices.GetService<IConfiguration>()
				.GetOptions<ServiceStackOptions>(ServiceStackOptions.SectionName);

			if (options == null || options.License == null)
			{
				return builder;
			}

			Licensing.RegisterLicense(options.License);

			return builder;
		}
	}
}