using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWindsDateTime.Interfaces;
public interface IDateTimeZone
{
  DateTime DateTime { get; }
  string TimeZoneId { get; }
  DateTimeOffset DateTimeOffset { get; }
  DateTimeZone Date { get; }
  DateTimeZone AddDays(double value);
  DateTimeZone AddHours(double value);
  DateTimeZone AddMinutes(double value);
  DateTime GetDateTime(string timeZoneId);
  DateTimeZone ConvertTimeZone(string timeZoneId);
  DateTimeZone RoundToMinute();
  int CompareTo(DateTimeZone? other);
}
