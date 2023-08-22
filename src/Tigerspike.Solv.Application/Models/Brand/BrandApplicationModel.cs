using Newtonsoft.Json;

namespace Tigerspike.Solv.Application.Models
{
	public class BrandApplicationModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CompanyName { get; set; }
		public string Role { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Comment { get; set; }

		[JsonProperty(PropertyName = "g-recaptcha-response")]
		public string GoogleRecaptchaResponse { get; set; }
	}
}
