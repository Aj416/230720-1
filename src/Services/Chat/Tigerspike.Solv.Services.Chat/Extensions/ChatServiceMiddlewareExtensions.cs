using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Chat.Configuration;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Services.Chat.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ChatServiceMiddlewareExtensions
    {
        public static void ConfigureChatDynamoDb(this IApplicationBuilder builder, bool createMissingTables)
        {
            var serviceProvider = builder.ApplicationServices;
            var db = serviceProvider.GetService<IPocoDynamo>();
            var settingsAccessor = serviceProvider.GetService<IOptions<DynamoDbOptions>>();

            DynamoDbConfig.InitSchema(db, settingsAccessor.Value, createMissingTables);
        }

		public static void ApplyDynamoDbMigrations(this IApplicationBuilder builder)
		{
		}

    }
}