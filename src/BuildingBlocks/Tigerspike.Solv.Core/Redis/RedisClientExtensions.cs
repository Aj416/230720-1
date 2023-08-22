using System;
using System.Collections.Generic;
using Serilog;
using ServiceStack.Redis;

namespace Tigerspike.Solv.Core.Redis
{
	public static class RedisClientExtensions
	{

		public static T Get<T>(this IRedisClient client, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			var typedClient = client.As<T>();

			if (typedClient.ContainsKey(key))
			{
				try
				{
					return typedClient.GetValue(key);
				}
				catch (RedisException ex)
				{
					Log.Logger.Error(ex, "Redis exception while getting key {key}", key);
					throw;
				}
			}
			else
			{
				return default(T);
			}
		}

		public static T Set<T>(this IRedisClient client, string key, T value, TimeSpan? expireIn = null,
			DateTime? expireAt = null)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			var typedClient = client.As<T>();

			try
			{

				typedClient.SetValue(key, value);

				if (expireIn.HasValue)
				{
					typedClient.ExpireEntryIn(key, expireIn.Value);
				}
				else if (expireAt.HasValue)
				{
					typedClient.ExpireEntryAt(key, expireAt.Value);
				}
			}
			catch (RedisException ex)
			{
				Log.Logger.Error(ex, "Redis exception while setting key {key} and value {value}", key, value);
				throw;
			}

			return value;
		}

		public static List<T> GetList<T>(this IRedisClient client, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			var typedClient = client.As<T>();

			if (typedClient.ContainsKey(key))
			{
				try
				{
					return typedClient.GetAllItemsFromList(typedClient.Lists[key]);
				}
				catch (RedisException ex)
				{
					Log.Logger.Error(ex, "Redis exception while getting list with key {key}", key);
					throw;
				}
			}
			else
			{
				return null;
			}
		}

		public static List<T> SetList<T>(this IRedisClient client, string key, List<T> values,
			TimeSpan? expireIn = null,
			DateTime? expireAt = null)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			var typedClient = client.As<T>();

			try
			{

				foreach (var item in values)
				{
					typedClient.AddItemToList(typedClient.Lists[key], item);
				}

				if (expireIn.HasValue)
				{
					typedClient.ExpireEntryIn(key, expireIn.Value);
				}
				else if (expireAt.HasValue)
				{
					typedClient.ExpireEntryAt(key, expireAt.Value);
				}
			}
			catch (RedisException ex)
			{
				Log.Logger.Error(ex, "Redis exception while setting list with key {key} and values {values}", key,
					values);
				throw;
			}

			return values;
		}

		public static bool IsTimeToLiveSet(this IRedisClient client, string key)
		{
			var ttl = client.GetTimeToLive(key);
			return ttl != null && ttl != TimeSpan.MaxValue;
		}
	}
}