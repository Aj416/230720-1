using System;
using System.Globalization;
using Humanizer;
using Humanizer.Localisation;

namespace Tigerspike.Solv.Application.Models
{
	public class TicketSolvedNotificationModel
	{
		public long Number { get; set; }
		public string BrandLogoUrl { get; set; }
		public string CustomerFirstName { get; set; }
		public string AdvocateFirstName { get; set; }
		public string RateUrl { get; set; }
		public string ChatUrl { get; set; }
		public string Question { get; set; }
		public string QuestionSummary { get; set; }
		public string Subject { get; set; }
		public string Header { get; set; }
		public string Body { get; set; }
		public int ClosingTime { get; set; }
		public string TicketClosingTime => TimeSpan.FromMinutes(ClosingTime).Humanize(culture: Culture, maxUnit: TimeUnit.Hour);
		public CultureInfo Culture { get; set; }

	}
}