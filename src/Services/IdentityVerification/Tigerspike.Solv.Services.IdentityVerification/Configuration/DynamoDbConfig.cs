using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Services.IdentityVerification.Domain;

namespace Tigerspike.Solv.Services.IdentityVerification.Configuration
{
    public static class DynamoDbConfig
    {

        public static void InitSchema(IPocoDynamo db, DynamoDbOptions options, bool createMissingTables)
        {
            var identityCheckEntityType = typeof(IdentityCheck);
            identityCheckEntityType.AddAttributes(new AliasAttribute(options.Tables.IdentityVerificationCheck));

            db.RegisterTable(identityCheckEntityType);

            if (createMissingTables)
            {
                db.InitSchema();
            }
        }
    }
}