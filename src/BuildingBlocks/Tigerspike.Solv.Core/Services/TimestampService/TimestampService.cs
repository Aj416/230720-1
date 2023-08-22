using System;

namespace Tigerspike.Solv.Core.Services
{
	public class TimestampService : ITimestampService
	{
		private readonly DateTime _utcTimestamp = DateTime.UtcNow;

		/// <inheritdoc/>
		public DateTime GetUtcTimestamp()
		{
			return _utcTimestamp;
		}
	}
}