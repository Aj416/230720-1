using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Mvc;

namespace Tigerspike.Solv.Core.Services
{
	public class RecaptchaApiClient : IRecaptchaApiClient
	{
		private readonly HttpClient _client;
		private readonly string _ipAddress;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly GoogleRecaptchaOptions _googleRecaptchaOptions;

		public RecaptchaApiClient(HttpClient client,
			IOptions<GoogleRecaptchaOptions> googleRecaptchaOptionsAccessor,
			IWebHostEnvironment hostingEnvironment,
			IHttpContextAccessor httpContextAccessor)
		{
			_client = client;
			_googleRecaptchaOptions = googleRecaptchaOptionsAccessor.Value;
			_ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
			_hostingEnvironment = hostingEnvironment;
		}

		public async Task<bool> ValidateCaptcha(string recaptchaResponse)
		{
			if(!_googleRecaptchaOptions.EnableRecpatcha)
			{
				return true;
			}

			var response = await _client.GetStringAsync(
				$"https://www.google.com/recaptcha/api/siteverify?secret={_googleRecaptchaOptions.SecretKey}&response={recaptchaResponse}&remoteip={_ipAddress}");

			return ((dynamic) JObject.Parse(response)).success == "true";
		}
	}
}