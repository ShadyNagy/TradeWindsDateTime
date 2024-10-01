
// Copyright (c) 2024 Trade Winds Studios (David Thielen)
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

using System.Collections.Concurrent;

namespace TradeWindsDateTime
{
	/// <summary>
	/// Convert a date/time to/from UTC from/to AppUser.TimeZone. We use the static Create() because there's
	/// a limited number of these that will ever be asked for and they do not change.
	/// </summary>
	public class DateTimeConvertor
	{
		private readonly TimeZoneInfo _timeZoneInfo;

		private DateTimeConvertor(string? timeZoneId)
		{
			// if the user has not set a timezone, the "Local" is the server, not the browser.
			// but local is the best we can do.
			if (string.IsNullOrEmpty(timeZoneId))
				_timeZoneInfo = TimeZoneInfo.Local;
			else
				_timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
		}

		/// <summary>
		/// The current time in the timezone. Similar to DateTime.Now except it's in this object's
		/// specified timezone rather than the server timezone.
		/// </summary>
		public DateTime Now => ConvertFromUtc(DateTime.UtcNow);

		public DateTime ConvertToUtc(DateTime dateTime)
		{
			// DexExpress sets this in a lot of cases where it's not a UTC time. So we'll just ignore it.
			// if (dateTime.Kind == DateTimeKind.Utc)
			//		return dateTime;
			if (dateTime.Kind != DateTimeKind.Unspecified)
				dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
			return TimeZoneInfo.ConvertTimeToUtc(dateTime, _timeZoneInfo);
		}

		public DateTime ConvertFromUtc(DateTime dateTime)
		{
            if (dateTime.Kind != DateTimeKind.Utc)
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, _timeZoneInfo);
		}

		/// <summary>
		/// cache of the convertors by timezone.
		/// </summary>
		private static ConcurrentDictionary<string, DateTimeConvertor> Cache { get; } = new();

		/// <summary>
		/// Convertor for when the timezone is not set.
		/// </summary>
		private static DateTimeConvertor UndefinedTimeZoneConvertor { get; } = new(null);

		/// <summary>
		/// Get a DateTimeConvertor for the given timezone.
		/// </summary>
		/// <param name="timeZoneId"></param>
		/// <returns>The requested convertor.</returns>
		public static DateTimeConvertor Create(string? timeZoneId)
		{
			if (string.IsNullOrEmpty(timeZoneId))
				return UndefinedTimeZoneConvertor;

			if (Cache.TryGetValue(timeZoneId, out var convertor))
				return convertor;

			var cnv = new DateTimeConvertor(timeZoneId);
			Cache.TryAdd(timeZoneId, cnv);
			return cnv;
		}
	}
}
