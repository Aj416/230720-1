namespace Tigerspike.Solv.Core.Models
{
	public class JwtModel
	{
		public string Token { get; }

		public double Expires { get; }

		public JwtModel(string token, double expires)
		{
			Token = token;
			Expires = expires;
		}
	}
}