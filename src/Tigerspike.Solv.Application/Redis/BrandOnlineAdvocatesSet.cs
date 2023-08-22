using System;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Core.Redis
{
	public class BrandOnlineAdvocatesSet : GuidSet
	{
		private readonly ITimestampService _timestampService;

		public BrandOnlineAdvocatesSet(IRedisClient client, Guid brandId, ITimestampService timestampService)
			: base(client, CacheKeys.GetBrandOnlineAdvocatesKey(brandId))
		{
			_timestampService = timestampService;
		}

		public override void Add(Guid value)
		{
			base.Add(value);
			_client.ExpireEntryAt(_key, _timestampService.GetUtcTimestamp().Date.AddDays(1)); // expire this cache item at 00:00 UTC next day
		}
	}
}