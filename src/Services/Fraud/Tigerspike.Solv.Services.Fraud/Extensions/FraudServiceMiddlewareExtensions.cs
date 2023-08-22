using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Fraud.Configuration;
using Tigerspike.Solv.Services.Fraud.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Tigerspike.Solv.Services.Fraud.Extensions
{
	public static class FraudServiceMiddlewareExtensions
	{
		public static void ConfigureFraudDynamoDb(this IApplicationBuilder builder, bool createMissingTables)
		{
			var serviceProvider = builder.ApplicationServices;
			var db = serviceProvider.GetService<IPocoDynamo>();
			var settingsAccessor = serviceProvider.GetService<IOptions<DynamoDbOptions>>();

			DynamoDbConfig.InitSchema(db, settingsAccessor.Value, createMissingTables);
		}

		public static void ApplyDynamoDbMigrations(this IApplicationBuilder builder)
		{
			// this is a placeholder for DynamoDb migrations code
		}

	}
}