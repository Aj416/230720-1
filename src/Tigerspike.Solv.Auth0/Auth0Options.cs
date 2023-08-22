
namespace Tigerspike.Solv.Auth0
{
	public class Auth0Options
	{
		public const string SectionName = "Auth0";

		public Auth0ManagementApiOptions ManagementApi { get; set; }
	}

	public class Auth0ManagementApiOptions
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Audience { get; set; }
		public string Authority { get; set; }
	}
}
