using System;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Application.Redis
{
	public class AdvocateConnectionsSet : StringSet
	{
		private readonly ITimestampService _timestampService;

		public AdvocateConnectionsSet(IRedisClient client, Guid advocateId, ITimestampService timestampService)
			: base(client, CacheKeys.GetAdvocateConnectionsKey(advocateId))
		{
			_timestampService = timestampService;
		}

		public override void Add(string value)
		{
			base.Add(value);
			_client.ExpireEntryAt(_key, _timestampService.GetUtcTimestamp().Date.AddDays(1)); // expire this cache item at 00:00 UTC next day
		}
	}
}