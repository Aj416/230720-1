using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Redis;

namespace Tigerspike.Solv.Core.Redis
{
	public class GuidSet : IRedisSet<Guid>
	{
		protected readonly IRedisClient _client;
		protected readonly string _key;

		public GuidSet(IRedisClient client, string key)
		{
			_client = client;
			_key = key;
		}

		public long Count => _client.GetSetCount(_key);
		public virtual void Add(Guid value) => _client.AddItemToSet(_key, value.ToString());
		public void Remove(Guid value) => _client.RemoveItemFromSet(_key, value.ToString());
		public bool Contains(Guid value) => _client.SetContainsItem(_key, value.ToString());
		public IEnumerable<Guid> GetAll()
		{
			var items = _client.GetAllItemsFromSet(_key);
			return items.Select(Guid.Parse).ToHashSet();
		}
	}
}