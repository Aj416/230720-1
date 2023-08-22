using System;

namespace Tigerspike.Solv.Core.Services
{
	public interface ITimestampService
	{
		/// <summary>
		/// Returns UTC timestamp of the request (always the same for the particular http request)
		/// </summary>
		/// <returns>Full DateTime timestamp in UTC</returns>
		DateTime GetUtcTimestamp();
	}
}