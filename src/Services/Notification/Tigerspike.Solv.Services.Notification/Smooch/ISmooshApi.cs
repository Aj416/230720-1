using System.Threading.Tasks;
using Refit;

namespace Tigerspike.Solv.Services.Notification.Smooch
{
	public interface ISmooshApi
	{
		[Post("/v2/apps/{appId}/conversations/{conversationId}/messages")]
		[Headers("Content-Type:application/json")]
		Task<PostMessageResponse> PostMessage([Header("Authorization")] string token, [Query] string appId, [Query] string conversationId, [Body] PostMessageRequest input);
	}
}