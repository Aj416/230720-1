using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.WebHook.Domain;
using Tigerspike.Solv.Services.WebHook.Enums;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.WebHook.Infrastructure.Repositories
{
	/// <inheritdoc />
	public class SubscriptionRepository : ISubscriptionRepository
	{
		private readonly IPocoDynamo _db;

		/// <summary>
		/// SubscriptionRepository Parameterised constructor.
		/// </summary>
		public SubscriptionRepository(IPocoDynamo db) => _db = db;

		/// <inheritdoc />
		public Subscription AddOrUpdateSubscription(Subscription subscription) => _db.PutItem(subscription);

		/// <inheritdoc />
		public void DeleteSubscription(Guid brandId, Guid id) =>
			_db.DeleteItem<Subscription>(new DynamoId { Hash = brandId, Range = id});

		/// <inheritdoc />
		public Subscription GetSubscription(Guid brandId, Guid id)
		{
			return _db
				.FromQuery<Subscription>().KeyCondition($"BrandId = :brandId and Id = :id",
					new Dictionary<string, string>()
					{
						{"brandId", brandId.ToString()},
						{"id", id.ToString()}
					})
				.Exec().FirstOrDefault();
		}

		/// <inheritdoc />
		public Task<List<SubscriptionEventLocalIndex>> GetSubscriptions(Guid brandId, int eventType)
		{
			return _db
				.FromQueryIndex<SubscriptionEventLocalIndex>(s =>
					s.BrandId == brandId && s.WebHookEvent == eventType)
				.ExecAsync();
		}
	}
}