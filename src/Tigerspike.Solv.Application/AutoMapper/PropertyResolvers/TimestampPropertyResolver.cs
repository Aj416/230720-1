using System;
using AutoMapper;
using Tigerspike.Solv.Core.Services;

namespace Tigerspike.Solv.Application.AutoMapper
{
	public class TimestampPropertyResolver : IValueResolver<object, object, DateTime>
	{
		private readonly ITimestampService _timestampService;

		public TimestampPropertyResolver(ITimestampService timestampService)
		{
			_timestampService = timestampService;
		}

		public DateTime Resolve(object source, object destination, DateTime destMember, ResolutionContext context) => _timestampService.GetUtcTimestamp();
	}
}