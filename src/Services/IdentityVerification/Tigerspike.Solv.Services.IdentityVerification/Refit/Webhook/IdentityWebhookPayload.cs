namespace Tigerspike.Solv.Services.IdentityVerification.Refit.Webhook
{

	public class IdentityWebhookPayload
	{
		public ResourceType ResourceType { get; set; }
		public string Action { get; set; }
		public IdentityWebhookObject Object { get; set; }
	}
}