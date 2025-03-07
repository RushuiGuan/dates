﻿using System;
using System.Threading.Tasks;

namespace Albatross.Dates {
	public static partial class Extensions {
		/// <summary>
		/// Return the next weekday from a provided date.  If the provided date is Monday, the next 0 weekday is the same date and the 
		/// next 1 weekday would be Tuesday.  If the provided date falls on the weekend, the next 0 weekday and the next 1 weekday are both
		/// Monday.  If the provided date is Friday, the next 0 weekday would be the same day and the next 1 weekday would be Monday.
		/// This method use math to figure out the correct next weekday.  It is more performant compare to the NextBusinessDay implementation where
		/// it loops each day and check if it is a business day.
		/// </summary>
		/// <param name="date">the starting date</param>
		/// <param name="numberOfWeekDays">number of week days to count</param>
		/// <exception cref="ArgumentException">exception will be thrown if the numberOfWeekDays parameter is less than 0</exception>
		public static DateTime NextWeekday(this DateTime date, int numberOfWeekDays = 1) {
			if (numberOfWeekDays == 0) {
				if (date.DayOfWeek == DayOfWeek.Sunday) {
					return date.Date.AddDays(1);
				} else if (date.DayOfWeek == DayOfWeek.Saturday) {
					return date.Date.AddDays(2);
				} else {
					return date.Date;
				}
			} else if (numberOfWeekDays > 0) {
				if (date.DayOfWeek == DayOfWeek.Sunday) {
					date = date.AddDays(-2);
				} else if (date.DayOfWeek == DayOfWeek.Saturday) {
					date = date.AddDays(-1);
				}
				if (numberOfWeekDays >= 5) {
					var weeks = numberOfWeekDays / 5;
					date = date.Date.AddDays(7 * weeks);
				}
				var remaining = numberOfWeekDays % 5;
				if ((int)date.DayOfWeek + remaining > 5) {
					remaining = remaining + 2;
				}
				date = date.AddDays(remaining);
				return date;
			} else {
				throw new ArgumentException($"{nameof(numberOfWeekDays)} parameter has to be greater or equal to 0");
			}
		}
		/// <summary>
		/// Return the previous weekday from a provided date.  If the provided date is Monday, the previous 0 weekday is the same date and the 
		/// previous 1 weekday would be Friday.  If the provided date falls on the weekend, the previous 0 weekday and the previous 1 weekday are both
		/// Friday.  
		/// </summary>
		/// <param name="date"></param>
		/// <param name="numberOfWeekDays"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">exception will be thrown if the numberOfWeekDays parameter is less than 0</exception>
		public static DateTime PreviousWeekday(this DateTime date, int numberOfWeekDays = 1) {
			if (numberOfWeekDays == 0) {
				if (date.DayOfWeek == DayOfWeek.Sunday) {
					return date.Date.AddDays(-2);
				} else if (date.DayOfWeek == DayOfWeek.Saturday) {
					return date.Date.AddDays(-1);
				} else {
					return date.Date;
				}
			} else if (numberOfWeekDays > 0) {
				if (date.DayOfWeek == DayOfWeek.Sunday) {
					date = date.AddDays(1);
				} else if (date.DayOfWeek == DayOfWeek.Saturday) {
					date = date.AddDays(2);
				}
				if (numberOfWeekDays >= 5) {
					var weeks = numberOfWeekDays / 5;
					date = date.Date.AddDays(-7 * weeks);
				}
				var remaining = numberOfWeekDays % 5;
				if ((int)date.DayOfWeek - remaining < 1) {
					remaining = remaining + 2;
				}
				date = date.AddDays(-1 * remaining);
				return date;
			} else {
				throw new ArgumentException($"{nameof(numberOfWeekDays)} parameter has to be greater or equal to 0");
			}
		}

		public static bool IsWeekDay(this DateTime date) => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

		/// <summary>
		/// Similar to NextWeekDay method but the caller can pass in a predicate to check if a date is a business day.  The method is also async, because
		/// holiday data could comes from external resource.  This method is not performant since it has to check every day to see if it is a business day.
		/// </summary>
		/// <param name="date"></param>
		/// <param name="numberOfBusinessDays"></param>
		/// <param name="isBusinessDay"></param>
		/// <returns></returns>
		public static async Task<DateTime> NextBusinessDay(this DateTime date, int numberOfBusinessDays = 1, Func<DateTime, Task<bool>>? isBusinessDay = null) {
			if (isBusinessDay == null) { isBusinessDay = args => Task.FromResult(IsWeekDay(args)); }
			for (int i = 0; i < numberOfBusinessDays; i++) {
				date = date.AddDays(1);
				while (!await isBusinessDay(date)) {
					date = date.AddDays(1);
				}
			}
			while (!await isBusinessDay(date)) {
				date = date.AddDays(1);
			}
			return date;
		}
		public static async Task<DateTime> PreviousBusinessDay(this DateTime date, int numberOfBusinessDays = 1, Func<DateTime, Task<bool>>? isBusinessDay = null) {
			if (isBusinessDay == null) { isBusinessDay = args => Task.FromResult(IsWeekDay(args)); }
			for (int i = 0; i < numberOfBusinessDays; i++) {
				date = date.AddDays(-1);
				while (!await isBusinessDay(date)) {
					date = date.AddDays(-1);
				}
			}
			while (!await isBusinessDay(date)) {
				date = date.AddDays(-1);
			}
			return date;
		}
		/// <summary>
		/// find the nth day of week from the given date
		/// </summary>
		public static DateTime GetNthDayOfWeek(this DateTime date, int n, DayOfWeek dayOfWeek) {
			var diff = dayOfWeek - date.DayOfWeek;
			if (diff < 0) {
				diff = 7 - System.Math.Abs(diff);
			}
			return date.AddDays(diff + (n - 1) * 7);
		}

		public static int GetMonthDiff(this DateTime date1, DateTime date2)
			=> ((date2.Year - date1.Year) * 12) + date2.Month - date1.Month;

		public static int GetNumberOfWeekdays(this DateTime d1, DateTime d2) {
			DateTime startDate, endDate;
			if (d1 < d2) {
				startDate = d1;
				endDate = d2;
			} else {
				startDate = d2;
				endDate = d1;
			}

			int totalDays = Convert.ToInt32(Math.Round((endDate.Date - startDate.Date).TotalDays, 0)) + 1;
			int completeWeeks = totalDays / 7;
			int weekdays = completeWeeks * 5;

			// Calculate the remaining days
			int remainingDays = totalDays % 7;

			// Iterate over the remaining days, checking if each one is a weekday
			for (int i = 0; i < remainingDays; i++) {
				DayOfWeek day = startDate.AddDays(i).DayOfWeek;
				if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
					weekdays++;
			}
			return weekdays;
		}


		public static DateTime Local2Utc(this DateTime local, TimeSpan gmtOffset) => DateTime.SpecifyKind(local - gmtOffset, DateTimeKind.Utc);
		public static DateTime Utc2Local(this DateTime utc, TimeSpan gmtOffset) => DateTime.SpecifyKind(utc + gmtOffset, DateTimeKind.Unspecified);
		public static DateTimeOffset Utc2DateTimeOffset(this DateTime utc, TimeSpan gmtOffset) => new DateTimeOffset(utc.Utc2Local(gmtOffset), gmtOffset);
		public static TimeSpan GmtOffset(this DateTime utc, DateTime local) => local - utc;

		public static DateTimeOffset Local2DateTimeOffset(this DateTime local, TimeZoneInfo timeZone) {
			local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
			var gmtOffset = timeZone.GetUtcOffset(local);
			return new DateTimeOffset(local, gmtOffset);
		}
		public static DateTimeOffset Utc2DateTimeOffset(this DateTime utc, TimeZoneInfo timeZone) {
			var gmtOffset = timeZone.GetUtcOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
			return new DateTimeOffset(utc.Utc2Local(gmtOffset), gmtOffset);
		}

		public static string ISO8601StringDateOnly(this DateTime value) => value.ToString(Formatting.ISO8601DateOnly);
		public static string ISO8601String(this DateTime value) => value.ToString(Formatting.ISO8601);
		public static string ISO8601String(this DateTimeOffset value) => value.ToString(Formatting.ISO8601);

		public static DateTime StartOfMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1);
		public static DateTime EndOfMonth(this DateTime date) => new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
	}
}