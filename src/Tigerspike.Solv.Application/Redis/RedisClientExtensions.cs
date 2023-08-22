using System;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Application.Redis
{
	public static class RedisDomainExtensions
	{

		public static IRedisSet<string> AdvocateConnections(this IRedisClient client, ITimestampService timestampService, Guid advocateId) => new AdvocateConnectionsSet(client, advocateId, timestampService);
		public static IRedisSet<Guid> BrandOnlineAdvocates(this IRedisClient client, ITimestampService timestampService, Guid brandId) => new BrandOnlineAdvocatesSet(client, brandId, timestampService);

	}
}