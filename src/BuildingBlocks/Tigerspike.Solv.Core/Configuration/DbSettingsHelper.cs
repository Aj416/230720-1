using System;
using System.IO;
using ConfigurationSubstitution;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Tigerspike.Solv.Core.Configuration
{
	public class DbSettingsHelper
	{
		private MySqlConnectionStringBuilder _connectionStringBuilder;

		public DbSettingsHelper()
		{
			// Get environment
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var configuration = new ConfigurationBuilder()
				.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Tigerspike.Solv.Api"))
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment}.json", optional: true)
				.AddEnvironmentVariables()
				.EnableSubstitutions("${", "}")
				.Build();

			SetFromConfiguration(configuration);
		}

		public DbSettingsHelper(IConfiguration configuration) => SetFromConfiguration(configuration);

		public DbSettingsHelper(DatabaseOptions settings) => SetConnectionString(settings);

		/// <summary>
		/// Sets the AllowUserVariables in the connection string.
		/// </summary>
		/// <param name="value">The value</param>
		public void SetAllowUserVariables(bool value) => _connectionStringBuilder.AllowUserVariables = value;

		/// <summary>
		/// Gets the connection string from the configuration.
		/// </summary>
		/// <returns>The connection string.</returns>
		public string GetConnectionString() => _connectionStringBuilder.ConnectionString;

		private void SetFromConfiguration(IConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			var dbSettings = configuration.GetSection(DatabaseOptions.SectionName);

			_connectionStringBuilder = new MySqlConnectionStringBuilder
			{
				Database = dbSettings[nameof(DatabaseOptions.Database)],
				Server = dbSettings[nameof(DatabaseOptions.Server)],
				UserID = dbSettings[nameof(DatabaseOptions.User)],
				Password = dbSettings[nameof(DatabaseOptions.Password)],
				Port = uint.Parse(dbSettings[nameof(DatabaseOptions.Port)]),
				UseAffectedRows = false
			};
		}

		private void SetConnectionString(DatabaseOptions settings)
		{
			_connectionStringBuilder = new MySqlConnectionStringBuilder
			{
				Database = settings.Database,
				Server = settings.Server,
				UserID = settings.User,
				Password = settings.Password,
				Port = settings.Port,
				UseAffectedRows = false,
				TreatTinyAsBoolean = false
			};
		}
	}
}