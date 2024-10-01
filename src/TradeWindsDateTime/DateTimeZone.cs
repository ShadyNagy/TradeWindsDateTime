using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeWindsDateTime.Interfaces;

namespace TradeWindsDateTime;

public class DateTimeZone : IComparable<DateTimeZone>, IDateTimeZone
{
  public static readonly string UtcTimeZone = TimeZoneInfo.Utc.Id;

  private readonly DateTime _dateTime;
  private TimeZoneInfo? _timeZoneInfo;

  [MaxLength(34)]
  public string TimeZoneId { get; private init; }

  public DateTime DateTime => _dateTime;

  [NotMapped]
  public TimeZoneInfo TimeZoneInfo
  {
    get
    {
      if (_timeZoneInfo == null)
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
      return _timeZoneInfo;
    }
  }

  public DateTimeOffset DateTimeOffset => new(DateTime, TimeZoneInfo.GetUtcOffset(DateTime));

  /// <summary>
  /// returns the date part of the DateTime in the TimeZoneId.
  /// </summary>
  public DateTimeZone Date => new(DateTime.Date, TimeZoneId);

  /// <summary>
  /// Returns the DateTime field converted to UTC.
  /// </summary>
  public DateTime UtcDateTime => TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo);

  /// <summary>
  /// Create a new DateTimeZone with the current time and the passed in TimeZoneId.
  /// </summary>
  /// <param name="dateTime">The DateTime. DateTimeKind will be silently set to Unspecified.
  /// This is the DateTime in the passed in TimeZoneId.</param>
  /// <param name="timeZoneId">The TimeZone Id.</param>
  public DateTimeZone(DateTime dateTime, string timeZoneId)
  {
    TimeZoneId = GetSafeTimeZoneId(timeZoneId);
    _dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
  }

  /// <summary>
  /// Create the object with the current time and the local timezone.
  /// </summary>
  public DateTimeZone() : this(DateTime.Now, TimeZoneInfo.Local.Id) { }

  /// <summary>
  /// Copy constructor.
  /// </summary>
  /// <param name="src">Populate from this object's values.</param>
  public DateTimeZone(DateTimeZone src) : this(src._dateTime, src.TimeZoneId) { }

  /// <summary>
  /// Returns a new DateTimeZone that adds the specified number of days to the value of this instance.
  /// </summary>
  /// <param name="value">A number of whole and fractional days. The value parameter can be negative or
  /// positive.</param>
  /// <returns>An object whose value is the sum of the date and time represented by this instance and the number
  /// of days represented by value.</returns>
  public DateTimeZone AddDays(double value) => new(DateTime.AddDays(value), TimeZoneId);

  /// <summary>
  /// Returns a new DateTimeZone that adds the specified number of hours to the value of this instance.
  /// </summary>
  /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or
  /// positive.</param>
  /// <returns>An object whose value is the sum of the date and time represented by this instance and the number
  /// of hours represented by value.</returns>
  public DateTimeZone AddHours(double value) => new(DateTime.AddHours(value), TimeZoneId);

  /// <summary>
  /// Returns a new DateTimeZone that adds the specified number of minutes to the value of this instance.
  /// </summary>
  /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or
  /// positive.</param>
  /// <returns>An object whose value is the sum of the date and time represented by this instance and the number
  /// of minutes represented by value.</returns>
  public DateTimeZone AddMinutes(double value) => new(DateTime.AddMinutes(value), TimeZoneId);

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
  /// Create and return a new object that is converted to the passed in TimeZoneId. This <b>will</b> adjust
  /// the DateTime property by the difference in the time zones.
  /// </summary>
  /// <param name="timeZoneId">Adjust the returned datetime to the matching time in this timezone.</param>
  /// <returns>The DateTime in the requested timezone.</returns>
  public DateTimeZone ConvertTimeZone(string timeZoneId)
  {
    if (timeZoneId == TimeZoneId) return this;
    timeZoneId = GetSafeTimeZoneId(timeZoneId);

    var destTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    var convertedDateTime = TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, destTimeZoneInfo);
    return new DateTimeZone(convertedDateTime, timeZoneId);
  }

  /// <summary>
  /// Round to the nearest minute.
  /// </summary>
  /// <returns>The rounded value.</returns>
  public DateTimeZone RoundToMinute()
  {
    var truncated = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, _dateTime.Minute, 0, _dateTime.Kind);
    if (_dateTime.Second >= 30)
      truncated = truncated.AddMinutes(1);
    return new DateTimeZone(truncated, TimeZoneId);
  }

  private static string GetSafeTimeZoneId(string? timeZoneId) => string.IsNullOrEmpty(timeZoneId) ? UtcTimeZone : timeZoneId;

  /// <inheritdoc />
  public int CompareTo(DateTimeZone? other)
  {
    if (ReferenceEquals(this, other)) return 0;
    if (ReferenceEquals(null, other)) return 1;

    // if they have the same timezone, compare the dates
    if (TimeZoneId == other.TimeZoneId)
      return _dateTime.CompareTo(other._dateTime);

    // ok, convert both to UTC and then compare the dates.
    var utc1 = ConvertTimeZone(UtcTimeZone);
    var utc2 = other.ConvertTimeZone(UtcTimeZone);
    return utc1.CompareTo(utc2);
  }

  /// <inheritdoc />
  public override bool Equals(object? obj) => obj is DateTimeZone other && CompareTo(other) == 0;

  /// <inheritdoc />
  public override int GetHashCode() => HashCode.Combine(_dateTime, TimeZoneId);

  /// <inheritdoc />
  public override string ToString() => $"{nameof(DateTime)}: {_dateTime}, {nameof(TimeZoneId)}: {TimeZoneId}";

  public static bool operator <(DateTimeZone a, DateTimeZone b) => a.CompareTo(b) < 0;
  public static bool operator <=(DateTimeZone a, DateTimeZone b) => a.CompareTo(b) <= 0;
  public static bool operator >(DateTimeZone a, DateTimeZone b) => a.CompareTo(b) > 0;
  public static bool operator >=(DateTimeZone a, DateTimeZone b) => a.CompareTo(b) >= 0;
  public static bool operator ==(DateTimeZone a, DateTimeZone b) => a.CompareTo(b) == 0;
  public static bool operator !=(DateTimeZone a, DateTimeZone b) => a.CompareTo(b) != 0;

  /// <summary>
  /// Returns with the DateTime set to Now in the passed in TimeZoneId.
  /// </summary>
  /// <param name="timeZoneId">Return Now in this timezone.</param>
  /// <returns>DateTime.Now in the requested timezone.</returns>
  public static DateTimeZone Now(string timeZoneId)
  {
    timeZoneId = GetSafeTimeZoneId(timeZoneId);
    return new DateTimeZone(DateTime.Now, timeZoneId).ConvertTimeZone(timeZoneId);
  }

  /// <summary>
  /// Returns with the DateTime set to UtcNow.
  /// </summary>
  public static DateTimeZone UtcNow => new(DateTime.UtcNow, UtcTimeZone);

  public static TimeSpan operator -(DateTimeZone a, DateTimeZone b) => a.UtcDateTime - b.UtcDateTime;
  public static DateTimeZone operator +(DateTimeZone a, TimeSpan b) => new DateTimeZone(a.DateTime + b, a.TimeZoneId);
}
