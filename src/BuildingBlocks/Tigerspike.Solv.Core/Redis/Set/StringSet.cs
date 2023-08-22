using System.Collections.Generic;
using ServiceStack.Redis;

namespace Tigerspike.Solv.Core.Redis
{
	public class StringSet : IRedisSet<string>
	{
		protected readonly IRedisClient _client;
		protected readonly string _key;

		public StringSet(IRedisClient client, string key)
		{
			_client = client;
			_key = key;
		}

		public long Count => _client.GetSetCount(_key);
		public virtual void Add(string value) => _client.AddItemToSet(_key, value);
		public void Remove(string value) => _client.RemoveItemFromSet(_key, value);
		public bool Contains(string value) => _client.SetContainsItem(_key, value);
		public IEnumerable<string> GetAll() => _client.GetAllItemsFromSet(_key);
	}
}