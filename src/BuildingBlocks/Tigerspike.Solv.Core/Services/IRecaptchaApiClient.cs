using System.Threading.Tasks;

namespace Tigerspike.Solv.Core.Services
{
	public interface IRecaptchaApiClient
	{
		Task<bool> ValidateCaptcha(string recaptchaResponse);
	}
}