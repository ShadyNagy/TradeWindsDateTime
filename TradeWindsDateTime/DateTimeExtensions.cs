
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

namespace TradeWindsDateTime
{
	/// <summary>
	/// Extensions to DateTime.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns the DateDiff between two dates. Does an Abs(diff) so the result is always positive.
		/// </summary>
		/// <param name="date1">This DateTime.</param>
		/// <param name="date2">The comparison DateTime</param>
		/// <returns>The difference.</returns>
		public static DateTimeSpan DateDiff(this DateTime date1, DateTime date2)
		{
			return DateTimeSpan.Diff(date1, date2);
		}

		/// <summary>
		/// Round to the nearest minute.
		/// </summary>
		/// <param name="dt">The DateTime to round.</param>
		/// <returns>The rounded value.</returns>
		public static DateTime RoundToMinute(this DateTime dt)
		{
			var truncated = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);
			return dt.Second >= 30 ? truncated.AddMinutes(1) : truncated;
		}
	}
}
