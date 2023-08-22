using System;

namespace Tigerspike.Solv.Core.Services
{
	public class FixedTimestampService : ITimestampService
	{
		private readonly DateTime _utcTimestamp;

		public FixedTimestampService(DateTime value)
		{
			_utcTimestamp = value;
		}

		/// <inheritdoc/>
		public DateTime GetUtcTimestamp()
		{
			return _utcTimestamp;
		}

	}
}