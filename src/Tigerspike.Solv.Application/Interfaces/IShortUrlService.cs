using System.Threading.Tasks;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IShortUrlService
	{
		/// <summary>
		/// Creates a short url from a long url.
		/// </summary>
		/// <param name="longUrl">The long url.</param>
		/// <returns>The short url.</returns>
		public Task<string> GetShortUrl(string longUrl);
	}
}