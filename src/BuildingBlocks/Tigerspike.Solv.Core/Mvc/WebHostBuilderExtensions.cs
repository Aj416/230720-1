using System.Collections.Generic;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Tigerspike.Solv.Core.Mvc
{
	public static class WebHostBuilderExtensions
	{
		public static IWebHostBuilder ConfigureAppConfiguration(this IWebHostBuilder webHostBuilder)
		{
			webHostBuilder.ConfigureAppConfiguration((ctx, builder) =>
			{
				builder.AddInMemoryCollection(new[]
					{new KeyValuePair<string, string>("environment", ctx.HostingEnvironment.EnvironmentName)});
				builder.EnableSubstitutions("${", "}");
			});

			return webHostBuilder;
		}
	}
}