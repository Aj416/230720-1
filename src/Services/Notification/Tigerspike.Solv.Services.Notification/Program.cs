using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Tigerspike.Solv.Core.Logging;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Services.Notification
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration()
				.ConfigureKestrel(options => { options.AddServerHeader = false; })
				.UseStartup<Startup>()
				.UseLogging();
	}
}