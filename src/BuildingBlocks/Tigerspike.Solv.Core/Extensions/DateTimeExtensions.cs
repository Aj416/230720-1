using System;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns date of the start of the week
		/// </summary>
		/// <param name="timestamp">Timestamp to calculate start of the week for</param>
		/// <param name="startOfWeek">On what weekday the week starts</param>
		/// <returns>Date of the start of the week</returns>
		public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
		{
			int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;

			return DateTime.SpecifyKind(dt.AddDays(-1 * diff).Date,
							DateTimeKind.Utc);

		}

		/// <summary>
		///     A DateTime extension method that return a DateTime of the first day of the year with the time set to
		///     "00:00:00:000". The first moment of the first day of the year.
		/// </summary>
		/// <param name="dateTime">The dateTime to act on.</param>
		/// <returns>A DateTime of the first day of the year with the time set to "00:00:00:000".</returns>
		public static DateTime StartOfYear(this DateTime dateTime)
		{
			return DateTime.SpecifyKind(new DateTime(dateTime.Year, 1, 1), DateTimeKind.Utc);
		}

		/// <summary>
		/// A DateTime extension method that return a DateTime of the first day of the month with the time set to
		/// "00:00:00:000". The first moment of the first day of the month.
		/// </summary>
		/// <param name="dateTime">The dateTime to act on.</param>
		/// <returns>A DateTime of the first day of the month with the time set to "00:00:00:000".</returns>
		public static DateTime StartOfMonth(this DateTime dateTime)
		{
			return DateTime.SpecifyKind(new DateTime(dateTime.Year, dateTime.Month, 1), DateTimeKind.Utc);
		}

		/// <summary>
		/// Returns date of the start of the previous week
		/// </summary>
		/// <param name="dateTime">Timestamp to calculate start of the week for</param>
		/// <param name="startOfWeek">On what weekday the week starts</param>
		/// <returns>Date of the start of the previous week</returns>
		public static DateTime StartOfPreviousWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
		{
			return dateTime.StartOfWeek(startOfWeek).AddDays(-7);
		}
	}
}