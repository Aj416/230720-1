using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Services.Invoicing.Context
{
	public class MigrateHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IWebHostEnvironment _env;

		public MigrateHostedService(
			IServiceProvider serviceProvider,
			IWebHostEnvironment env)
		{
			_serviceProvider = serviceProvider;
			_env = env;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// Create a new scope to retrieve scoped services
			using var scope = _serviceProvider.CreateScope();
			if (_env.IsDev() || _env.IsLocal() || _env.IsDocker())
			{

				// Run the database migration automatically for local and development environments
				var serviceScopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
				await MigrateDatabase(serviceScopeFactory);
			}
		}
		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


		public static async Task MigrateDatabase(IServiceScopeFactory serviceScopeFactory)
		{
			using var scope = serviceScopeFactory.CreateScope();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<MigrateHostedService>>();

			var database = scope.ServiceProvider.GetService<InvoicingDbContext>().Database;
			logger.LogInformation($"Running system migrations for database {database.GetDbConnection().Database}");
			await database.MigrateAsync();
		}
	}
}