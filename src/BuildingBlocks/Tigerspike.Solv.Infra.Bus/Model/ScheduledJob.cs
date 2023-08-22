using System;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Core.Bus;

namespace Tigerspike.Solv.Infra.Bus.Model
{
	public class ScheduledJob : IScheduledJob
	{
		[HashKey]
		public string JobId { get; set; }
		public Guid Token { get; set; }
		public long ExpirationTimestamp { get; set; }

		public ScheduledJob()
		{

		}

		public ScheduledJob(string jobId, Guid token, DateTime expirationTimestamp)
		{
			JobId = jobId;
			Token = token;
			ExpirationTimestamp = new DateTimeOffset(expirationTimestamp).ToUnixTimeSeconds();
		}
	}
}