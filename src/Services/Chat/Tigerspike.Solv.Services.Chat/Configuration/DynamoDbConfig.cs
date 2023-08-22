using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Core.DynamoDb;
using Tigerspike.Solv.Infra.Bus.Model;

namespace Tigerspike.Solv.Chat.Configuration
{
    public static class DynamoDbConfig
    {

        public static void InitSchema(IPocoDynamo db, DynamoDbOptions options, bool createMissingTables)
        {
            var conversationEntityType = typeof(Conversation);
            conversationEntityType.AddAttributes(new AliasAttribute(options.Tables.Conversation));

            var messageEntityType = typeof(Message);
            messageEntityType.AddAttributes(new AliasAttribute(options.Tables.Message));

            var messageWhitelistEntityType = typeof(MessageWhitelist);
            messageWhitelistEntityType.AddAttributes(new AliasAttribute(options.Tables.MessageWhitelist));

            var scheduledJobType = typeof(ScheduledJob);
            scheduledJobType.AddAttributes(new AliasAttribute(options.Tables.ScheduledJob));

            var actionType = typeof(Action);
            actionType.AddAttributes(new AliasAttribute(options.Tables.ChatAction));

            var actionOptionType = typeof(ActionOption);
            actionOptionType.AddAttributes(new AliasAttribute(options.Tables.ChatAction));

            db.RegisterTable(conversationEntityType);
            db.RegisterTable(messageEntityType);
            db.RegisterTable(messageWhitelistEntityType);
            db.RegisterTable(scheduledJobType);
            db.RegisterTable(actionType);
            db.RegisterTable(actionOptionType);

            if (createMissingTables)
            {
                db.InitSchema();
            }
        }
    }
}