using Newtonsoft.Json;

namespace Tigerspike.Solv.Paypal.API
{
	public class AuthToken
	{
		[JsonProperty("scope")]
		public string Scope { get; set; }

		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("id_token")]
		public string IdToken { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }
	}
}