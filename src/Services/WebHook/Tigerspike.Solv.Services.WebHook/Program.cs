using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Tigerspike.Solv.Core.Logging;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Services.WebHook
{
	public class Program
	{
		public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

		public static IWebHostBuilder CreateHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration()
				.ConfigureKestrel(options => { options.AddServerHeader = false; })
				.UseStartup<Startup>()
				.UseLogging();
	}
}
