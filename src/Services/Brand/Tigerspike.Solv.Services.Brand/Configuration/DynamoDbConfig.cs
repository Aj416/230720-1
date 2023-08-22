using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Core.DynamoDb;

namespace Tigerspike.Solv.Services.Brand.Configuration
{
    public static class DynamoDbConfig
    {

        public static void InitSchema(IPocoDynamo db, DynamoDbOptions options, bool createMissingTables)
        {
	        if (createMissingTables)
            {
                db.InitSchema();
            }
        }
    }
}