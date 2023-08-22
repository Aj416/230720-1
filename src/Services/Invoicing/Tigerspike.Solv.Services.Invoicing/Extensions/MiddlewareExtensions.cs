using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Invoicing.Configuration;

namespace Tigerspike.Solv.Services.Invoicing.Extensions
{
	public static class MiddlewareExtensions
	{
		public static void ConfigureDynamoDb(this IApplicationBuilder builder, bool createMissingTables)
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
