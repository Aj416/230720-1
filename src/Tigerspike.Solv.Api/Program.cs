using System.Collections.Generic;
using ConfigurationSubstitution;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Tigerspike.Solv.Core.Logging;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.RateLimiting;

namespace Tigerspike.Solv.Api
{
	/// <summary>
	/// Program Class
	/// </summary>
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().SeedRateLimitingRules().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration()
				.ConfigureKestrel(options => { options.AddServerHeader = false; })
				.UseStartup<Startup>()
				.UseLogging();
	}
}