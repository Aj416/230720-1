using System;
using System.Threading.Tasks;
using Refit;
using Tigerspike.Solv.Application.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class ShortUrlService : IShortUrlService
	{
		private readonly ITinyUrlApi _api;

		public ShortUrlService()
		{
			_api = RestService.For<ITinyUrlApi>("https://tinyurl.com");
		}

		/// <inheritdoc />
		public Task<string> GetShortUrl(string longUrl)
		{
			try
			{
				return _api.GetShortUrl(longUrl);
			}
			catch (Exception)
			{
				return Task.FromResult(longUrl);
			}
		}
	}

	public interface ITinyUrlApi
	{
		[Get("/api-create.php?url={longUrl}")]
		[Headers("Content-Type:application/json")]
		Task<string> GetShortUrl([Query] string longUrl);
	}
}