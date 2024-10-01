
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeWindsDateTime;
/// <summary>
/// A combined date and time with a time zone offset. The DateTime whenever passed in, if not set to Unspecified,
/// is silently converted to Unspecified, not changing the DateTime value. This object is immutable.
/// </summary>
public class DateTimeZone : IComparable<DateTimeZone>
{
  /// <summary>
  /// The timezone id for UTC.
  /// </summary>
  public static string UtcTimeZone = TimeZoneInfo.Utc.Id;

  private readonly DateTime _dateTime;

  private TimeZoneInfo? _timeZoneInfo;

  /// <summary>
  /// The DateTime. This is always of DateTimeKind.Unspecified. This is the DateTime in the
  /// TimeZoneId property timezone.
  /// </summary>
  public DateTime DateTime
  {
    get => _dateTime;
    private init => _dateTime = value.Kind == DateTimeKind.Unspecified ? value : DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
  }

  /// <summary>
  /// This object as a DateTimeOffset. This uses the DateTime and TimeZoneInfo properties and calculates the offset for the
  /// Date part of DateTime.
  /// </summary>
  public DateTimeOffset DateTimeOffset => new(DateTime, TimeZoneInfo.GetUtcOffset(DateTime));

  /// <summary>
  /// The TimeZoneId. These are defined in https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.id
  /// </summary>
  [MaxLength(34)]
  public string TimeZoneId { get; private init; }

  /// <summary>
  /// The TimeZoneInfo for the TimeZoneId. This is created on the fly and is not stored in the database as its
  /// underlying data can change when timezone rules are changed.
  /// </summary>
  [NotMapped]
  public TimeZoneInfo TimeZoneInfo
  {
    get
    {
      if (_timeZoneInfo is null)
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
      return _timeZoneInfo;
    }
  }

  /// <summary>
  /// Returns the DateTime field converted to UTC.
  /// </summary>
  public DateTime UtcDateTime => TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo);

  /// <summary>
  /// returns the date part of the DateTime in the TimeZoneId.
  /// </summary>
  public DateTimeZone Date => new(DateTime.Date, TimeZoneId);

  /// <summary>
  /// Returns a new DateTimeZone that adds the specified number of days to the value of this instance.
  /// </summary>
  /// <param name="value">A number of whole and fractional days. The value parameter can be negative or
  /// positive.</param>
  /// <returns>An object whose value is the sum of the date and time represented by this instance and the number
  /// of days represented by value.</returns>
  public DateTimeZone AddDays(double value)
  {
    return new DateTimeZone(DateTime.AddDays(value), TimeZoneId);
  }

  /// <summary>
  /// Returns a new DateTimeZone that adds the specified number of hours to the value of this instance.
  /// </summary>
  /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or
  /// positive.</param>
  /// <returns>An object whose value is the sum of the date and time represented by this instance and the number
  /// of hours represented by value.</returns>
  public DateTimeZone AddHours(double value)
  {
    return new DateTimeZone(DateTime.AddHours(value), TimeZoneId);
  }

  /// <summary>
  /// Returns a new DateTimeZone that adds the specified number of minutes to the value of this instance.
  /// </summary>
  /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or
  /// positive.</param>
  /// <returns>An object whose value is the sum of the date and time represented by this instance and the number
  /// of minutes represented by value.</returns>
  public DateTimeZone AddMinutes(double value)
  {
    return new DateTimeZone(DateTime.AddMinutes(value), TimeZoneId);
  }

  /// <summary>
  /// Return the DateTime in the passed in TimeZoneId.
  /// </summary>
  /// <param name="timeZoneId">Convert to the datetime in this timezone</param>
  /// <returns>The datetime in the passed in timezone.</returns>
  public DateTime GetDateTime(string timeZoneId)
  {
    timeZoneId = GetSafeTimeZoneId(timeZoneId);
    var destTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    return TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, destTimeZoneInfo);
  }

  /// <summary>
  /// Create a new DateTimeZone with the current time and the passed in TimeZoneId.
  /// </summary>
  /// <param name="dateTime">The DateTime. DateTimeKind will be silently set to Unspecified.
  /// This is the DateTime in the passed in TimeZoneId.</param>
  /// <param name="timeZoneId">The TimeZone Id.</param>
  public DateTimeZone(DateTime dateTime, string timeZoneId)
  {
    timeZoneId = GetSafeTimeZoneId(timeZoneId);
    if (dateTime.Kind != DateTimeKind.Unspecified)
      dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
    DateTime = dateTime;
    TimeZoneId = timeZoneId;
  }

  /// <summary>
  /// Create the object with the current time and the local timezone.
  /// </summary>
  public DateTimeZone()
  {
    DateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
    TimeZoneId = TimeZoneInfo.Local.Id;
  }

  /// <summary>
  /// Copy constructor.
  /// </summary>
  /// <param name="src">Populate from this object's values.</param>
  public DateTimeZone(DateTimeZone src)
  {
    _dateTime = src._dateTime;
    TimeZoneId = src.TimeZoneId;
  }

  /// <summary>
  /// Create and return a new object that is converted to the passed in TimeZoneId. This <b>will</b> adjust
  /// the DateTime property by the difference in the time zones.
  /// </summary>
  /// <param name="timeZoneId">Adjust the returned datetime to the matching time in this timezone.</param>
  /// <returns>The DateTime in the requested timezone.</returns>
  public DateTimeZone ConvertTimeZone(string timeZoneId)
  {
    if (timeZoneId == TimeZoneId)
      return this;
    timeZoneId = GetSafeTimeZoneId(timeZoneId);

    var destTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    var dateTime = TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, destTimeZoneInfo);
    return new DateTimeZone(dateTime, timeZoneId);
  }

  /// <summary>
  /// Returns with the DateTime set to Now in the passed in TimeZoneId.
  /// </summary>
  /// <param name="timeZoneId">Return Now in this timezone.</param>
  /// <returns>DateTime.Now in the requested timezone.</returns>
  public static DateTimeZone Now(string timeZoneId)
  {
    timeZoneId = GetSafeTimeZoneId(timeZoneId);
    var now = new DateTimeZone(DateTime.Now, TimeZoneInfo.Local.Id);
    return now.ConvertTimeZone(timeZoneId);
  }

  /// <summary>
  /// Returns with the DateTime set to UtcNow.
  /// </summary>
  public static DateTimeZone UtcNow => new(DateTime.UtcNow, UtcTimeZone);

  /// <summary>
  /// Round to the nearest minute.
  /// </summary>
  /// <returns>The rounded value.</returns>
  public DateTimeZone RoundToMinute()
  {
    var truncated = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, _dateTime.Minute, 0, _dateTime.Kind);
    if (_dateTime.Second >= 30)
      truncated = truncated.AddMinutes(1);
    return new DateTimeZone(truncated, _timeZoneInfo != null ? _timeZoneInfo.Id : TimeZoneInfo.Id);
  }

  private static string GetSafeTimeZoneId(string? timeZoneId)
  {
    if (string.IsNullOrEmpty(timeZoneId))
      return UtcTimeZone;
    return timeZoneId;
  }

  /// <inheritdoc />
  public int CompareTo(DateTimeZone? other)
  {
    if (ReferenceEquals(this, other))
      return 0;
    if (ReferenceEquals(null, other))
      return 1;

    // if they have the same timezone, compare the dates
    if (TimeZoneId == other.TimeZoneId)
      return _dateTime.CompareTo(other._dateTime);

    // ok, convert both to UTC and then compare the dates.
    var utc1 = ConvertTimeZone(UtcTimeZone);
    var utc2 = other.ConvertTimeZone(UtcTimeZone);
    return utc1.CompareTo(utc2);
  }

  public static bool operator <(DateTimeZone x, DateTimeZone y) => x.CompareTo(y) < 0;
  public static bool operator <=(DateTimeZone x, DateTimeZone y) => x.CompareTo(y) <= 0;
  public static bool operator >(DateTimeZone x, DateTimeZone y) => x.CompareTo(y) > 0;
  public static bool operator >=(DateTimeZone x, DateTimeZone y) => x.CompareTo(y) >= 0;

  public static bool operator !=(DateTimeZone x, DateTimeZone y) => x.CompareTo(y) != 0;
  public static bool operator ==(DateTimeZone x, DateTimeZone y) => x.CompareTo(y) == 0;

  protected bool Equals(DateTimeZone other)
  {
    return CompareTo(other) == 0;
  }

  /// <inheritdoc />
  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj))
      return false;
    if (ReferenceEquals(this, obj))
      return true;
    if (obj.GetType() != GetType())
      return false;
    return Equals((DateTimeZone)obj);
  }

  /// <inheritdoc />
  public override int GetHashCode()
  {
    return HashCode.Combine(_dateTime, TimeZoneId);
  }

  /// <inheritdoc />
  public override string ToString()
  {
    return $"{nameof(DateTime)}: {_dateTime}, {nameof(TimeZoneId)}: {TimeZoneId}";
  }

  public static TimeSpan operator -(DateTimeZone a, DateTimeZone b)
  {
    return a.UtcDateTime - b.UtcDateTime;
  }

  public static DateTimeZone operator +(DateTimeZone a, TimeSpan b)
  {
    var localDateTime = a.DateTime + b;
    return new DateTimeZone(localDateTime, a.TimeZoneId);
  }
}
