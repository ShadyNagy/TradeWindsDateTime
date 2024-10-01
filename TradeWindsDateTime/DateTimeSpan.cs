
// Much of the below code is from https://stackoverflow.com/a/9216404/509627
//
// All additional changes Copyright (c) 2024 Trade Winds Studios (David Thielen)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace TradeWindsDateTime
{

	/// <summary>
	/// Similar to TimeSpan but for a Date span.
	/// </summary>
	public readonly struct DateTimeSpan
	{
		/// <summary>
		/// The full years in the span. If the two dates are in adjacent years, less than 365 days apart, this will be 0.
		/// </summary>
		public int Years { get; }

		/// <summary>
		/// The full months in the span, after subtracting years. If the two dates are in adjacent months, less than the
		/// matching day of the month apart, this will be 0.
		/// </summary>
		public int Months { get; }

		/// <summary>
		/// The full days in the span, after subtracting years and months. If the two dates are in adjacent days, less
		/// than 24 hours apart, this will be 0.
		/// </summary>
		public int Days { get; }

		/// <summary>
		/// The full hours in the span, after subtracting years, months, and days. If the two dates are in adjacent hours,
		/// less than 1 hour apart, this will be 0.
		/// </summary>
		public int Hours { get; }

		/// <summary>
		/// The full minutes in the span, after subtracting years, months, days, and hours. If the two dates are in adjacent
		/// minutes, less than 1 minute apart, this will be 0.
		/// </summary>
		public int Minutes { get; }

		/// <summary>
		/// The full seconds in the span, after subtracting years, months, days, hours, and minutes. If the two dates are in
		/// adjacent seconds, less than 1 second apart, this will be 0.
		/// </summary>
		public int Seconds { get; }

		/// <summary>
		/// The full milliseconds in the span, after subtracting years, months, days, hours, minutes, and seconds. 
		/// </summary>
		public int Milliseconds { get; }

		public DateTimeSpan(DateTime src)
		{
			Years = src.Year;
			Months = src.Month;
			Days = src.Day;
			Hours = src.Hour;
			Minutes = src.Minute;
			Seconds = src.Second;
			Milliseconds = src.Millisecond;
		}

		/// <summary>
		/// The number of weeks in the partial month. This is the number of days divided by 7. If the day of the
		/// month is the same for both dates, this will be 0.
		/// </summary>
		public int WeeksInMonth => Days / 7;

		/// <summary>
		/// The number of days remaining after the full weeks are removed from the partial month. If the day of the
		/// month is the same for both dates, this will be 0.
		/// </summary>
		public int DaysRemainderWeeks => Days % 7;

		public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
		{
			Years = years;
			Months = months;
			Days = days;
			Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
			Milliseconds = milliseconds;
		}

		private enum Phase
		{
			Years,
			Months,
			Days,
			Done
		}

		/// <summary>
		/// The difference between two dates. Always returns a positive DateTimeSpan (in other words
		/// it's Abs(diff)).
		/// </summary>
		public static DateTimeSpan Diff(DateTime date1, DateTime date2)
		{
			if (date2 < date1)
				(date1, date2) = (date2, date1);

			var current = date1;
			var years = 0;
			var months = 0;
			var days = 0;

			var phase = Phase.Years;
			var span = new DateTimeSpan();
			var officialDay = current.Day;

			while (phase != Phase.Done)
			{
				switch (phase)
				{
					case Phase.Years:
						if (current.AddYears(years + 1) > date2)
						{
							phase = Phase.Months;
							current = current.AddYears(years);
						}
						else
							years++;

						break;
					case Phase.Months:
						if (current.AddMonths(months + 1) > date2)
						{
							phase = Phase.Days;
							current = current.AddMonths(months);
							if (current.Day < officialDay &&
								officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
								current = current.AddDays(officialDay - current.Day);
						}
						else
							months++;

						break;
					case Phase.Days:
						if (current.AddDays(days + 1) > date2)
						{
							current = current.AddDays(days);
							var timespan = date2 - current;
							span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes,
								timespan.Seconds, timespan.Milliseconds);
							phase = Phase.Done;
						}
						else
							days++;

						break;
				}
			}

			return span;
		}

		/// <summary>
		/// The time span using the two largest non-zero measures. For example "3 weeks, 2 days"
		/// or "3 hours, 5 minutes".
		/// </summary>
		/// <returns>The duration.</returns>
		public string DurationIn2()
		{
			if (Years != 0)
				return $"{Years} year{(Years == 1 ? "" : "s")}, {Months} month{(Months == 1 ? "" : "s")}";
			if (Months != 0 && WeeksInMonth > 0)
				return $"{Months} month{(Months == 1 ? "" : "s")}, {WeeksInMonth} week{(WeeksInMonth == 1 ? "" : "s")}";
			if (Months != 0)
				return $"{Months} month{(Months == 1 ? "" : "s")}, {Days} day{(Days == 1 ? "" : "s")}";
			if (WeeksInMonth > 0)
				return $"{WeeksInMonth} week{(WeeksInMonth == 1 ? "" : "s")}, {DaysRemainderWeeks} day{(DaysRemainderWeeks == 1 ? "" : "s")}";
			if (Days > 0)
				return $"{Days} day{(Days == 1 ? "" : "s")}, {Hours} hour{(Hours == 1 ? "" : "s")}";
			if (Hours > 0)
				return $"{Hours} hour{(Hours == 1 ? "" : "s")}, {Minutes} minute{(Minutes == 1 ? "" : "s")}";
			return $"{Minutes} minute{(Minutes == 1 ? "" : "s")}, {Seconds} second{(Seconds == 1 ? "" : "s")}";
		}
	}
}