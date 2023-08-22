using System;

namespace Tigerspike.Solv.Application.Services
{
	public class CacheKeyService : ICacheKeyService
	{
		public string PreviousWeekStatisticsPeriodKey(Guid advocateId) => $"advocate/{advocateId}/statistics/periods/week/previous";
		public string CurrentWeekStatisticsPeriodKey(Guid advocateId) => $"advocate/{advocateId}/statistics/periods/week/current";
		public string AllTimeStatisticsPeriodKey(Guid advocateId) => $"advocate/{advocateId}/statistics/periods/all-time";
	}
}

