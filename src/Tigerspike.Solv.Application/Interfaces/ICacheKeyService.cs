using System;

namespace Tigerspike.Solv.Application.Services
{
	public interface ICacheKeyService
	{
		string AllTimeStatisticsPeriodKey(Guid advocateId);
		string CurrentWeekStatisticsPeriodKey(Guid advocateId);
		string PreviousWeekStatisticsPeriodKey(Guid advocateId);
	}
}
