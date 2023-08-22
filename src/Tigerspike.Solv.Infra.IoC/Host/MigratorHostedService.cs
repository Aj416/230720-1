using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Infra.IoC.Host
{
	/// <summary>
	/// Responsible for running DB migration and seeding asynchrounsly at startup.
	/// </summary>
	public class MigratorHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IWebHostEnvironment _env;

		public MigratorHostedService(IServiceProvider serviceProvider, IWebHostEnvironment env)
		{
			_serviceProvider = serviceProvider;
			_env = env;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// Create a new scope to retrieve scoped services
			using (var scope = _serviceProvider.CreateScope())
			{
				if (_env.IsDev() || _env.IsLocal() || _env.IsDocker())
				{

					// Run the database migration automatically for local and development environments
					var serviceScopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
					await NativeInjectorBootStrapper.MigrateDatabase(serviceScopeFactory);
					await NativeInjectorBootStrapper.EnsureSeedData(serviceScopeFactory);
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
