using System;
using Humanizer;

namespace Tigerspike.Solv.Application.Models.Brand
{
	public class BrandResponseTemplateModel
	{
		public string AdvocateFirstName { get; private set; }
		public string CustomerFirstName { get; private set; }
		public string BrandName { get; set; }
		public TimeSpan? NextActionDelayTimeSpan { get; private set; }
		public string NextActionDelay => NextActionDelayTimeSpan?.Humanize();

		public BrandResponseTemplateModel(string advocateFirstName = null, string customerFirstName = null, TimeSpan? nextActionDelay = null, string brandName = null)
		{
			AdvocateFirstName = advocateFirstName;
			CustomerFirstName = customerFirstName;
			NextActionDelayTimeSpan = nextActionDelay;
			BrandName = brandName;
		}
	}
}
