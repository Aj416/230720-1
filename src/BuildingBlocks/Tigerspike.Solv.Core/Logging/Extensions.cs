using System;
using System.Collections.Generic;
using System.Linq;
using AWS.Logger;
using AWS.Logger.SeriLog;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Tigerspike.Solv.Core.Configuration;

namespace Tigerspike.Solv.Core.Logging
{
	public static class Extensions
	{
		private static string LoggingTemplate = @"[{Timestamp:HH:mm:ss} {Level:u3} ({SourceContext})] {Message:lj} - {Exception} {Properties:j} {NewLine}";

		public static IWebHostBuilder UseLogging(this IWebHostBuilder webHostBuilder, Action<LoggerConfiguration> configure = null)
			=> webHostBuilder.UseSerilog((context, loggerConfiguration) =>
			{
				var appOptions = context.Configuration.GetOptions<AppOptions>("App");
				var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("Serilog");

				MapOptions(serilogOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
				configure?.Invoke(loggerConfiguration);
			});

		private static void MapOptions(SerilogOptions loggerOptions, AppOptions appOptions,
			LoggerConfiguration loggerConfiguration, string environmentName)
		{
			var level = GetLogEventLevel(loggerOptions.Level);

			loggerConfiguration.MinimumLevel.Is(level)
				.Enrich.FromLogContext()
				.Enrich.WithExceptionDetails()
				.Enrich.WithProperty("Environment", environmentName)
				.Enrich.WithProperty("Application", appOptions.Name)
				.Enrich.WithProperty("Version", appOptions.Version);

			foreach (var (key, value) in loggerOptions.MinimumLevelOverrides ?? new Dictionary<string, string>())
			{
				var logLevel = GetLogEventLevel(value);
				loggerConfiguration.MinimumLevel.Override(key, logLevel);
			}

			loggerOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
				.ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

			loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
				.ByExcluding(Matching.WithProperty(p)));

			Configure(loggerConfiguration, level, loggerOptions);
		}

		private static void Configure(LoggerConfiguration loggerConfiguration, LogEventLevel level, SerilogOptions serilogOptions)
		{
			if (serilogOptions.Console != null && serilogOptions.Console.Enabled)
			{
				loggerConfiguration.WriteTo.Console(outputTemplate: LoggingTemplate);
			}

			if (serilogOptions.AWS != null && serilogOptions.AWS.Enabled)
			{
				var awsConfiguration = new AWSLoggerConfig(serilogOptions.AWS.LogGroup)
				{
					Region = serilogOptions.AWS.Region
				};
				loggerConfiguration.WriteTo.AWSSeriLog(awsConfiguration);
			}
		}

		private static LogEventLevel GetLogEventLevel(string level)
			=> Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
				? logLevel
				: LogEventLevel.Information;
	}
}