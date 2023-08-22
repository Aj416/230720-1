using Amazon.DynamoDBv2.DataModel;
using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Chat.Domain
{
    public class UserConversation
    {
        [HashKey]
        public string UserId { get; set; }

        [RangeKey]
        [DynamoDBGlobalSecondaryIndexHashKey("conversationId-index")]
        public string ConversationId { get; set; }
    }
}