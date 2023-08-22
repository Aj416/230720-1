using System;
using MassTransit;

namespace Tigerspike.Solv.Core.Bus
{
	public static class BusExtensions
	{
		/// <summary>
		/// Gets the queue uri required for scheduling messages.
		/// </summary>
		/// <param name="bus">The input bus.</param>
		/// <param name="queue">The queue name.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Uri GetQueueUri(this IBus bus, string queue)
		{
			if (bus == null)
			{
				throw new ArgumentNullException(nameof(bus));
			}

			return new UriBuilder(bus.Address) {Path = queue}.Uri;
		}
	}
}