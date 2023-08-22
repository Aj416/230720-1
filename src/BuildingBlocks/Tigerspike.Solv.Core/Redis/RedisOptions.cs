using System.Collections.Generic;
using System.Linq;
using System.Net;
using StackExchange.Redis;

namespace Tigerspike.Solv.Core.Redis
{
	public class RedisOptions
	{
		public const string SectionName = "Redis";

		public string  ReadWriteHosts { get; set; }

		public string ReadOnlyHosts { get; set; }

		public bool Ssl { get; set; }

		public EndPointCollection GetReadWriteEndpoints()
		{
			var endpoints = new EndPointCollection();

			foreach (var readWriteHost in GetReadWriteHosts())
			{
				endpoints.Add(readWriteHost);
			}

			return endpoints;
		}

		public List<string> GetReadWriteHosts()
		{
			return ReadWriteHosts
				.Split(',')
				.Where(n => !string.IsNullOrWhiteSpace(n))
				.Select(n => n.Trim())
				.ToList();
		}
	}
}