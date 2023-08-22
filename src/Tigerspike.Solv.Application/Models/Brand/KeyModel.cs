namespace Tigerspike.Solv.Application.Models
{
	public class ApiKeyModel
	{
		public string M2m { get; set; }
		public string Sdk { get; set; }

		public ApiKeyModel(string m2m, string sdk)
		{
			M2m = m2m;
			Sdk = sdk;
		}
	}
}
