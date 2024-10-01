namespace TradeWindsDateTime;

public class DateTimeSpan : IDateTimeSpan
{
  public int Years { get; private set; }
  public int Months { get; private set; }
  public int Days { get; private set; }
  public int Hours { get; private set; }
  public int Minutes { get; private set; }
  public int Seconds { get; private set; }
  public int Milliseconds { get; private set; }

  public int WeeksInMonth => Days / 7;
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

  public static IDateTimeSpan CalculateDifference(DateTime startDate, DateTime endDate)
  {
    if (endDate < startDate)
      (startDate, endDate) = (endDate, startDate);

    int years = CalculateYears(ref startDate, endDate);
    int months = CalculateMonths(ref startDate, endDate);
    int days = (endDate - startDate).Days;

    var timeSpan = endDate - startDate.AddDays(days);
    return new DateTimeSpan(years, months, days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
  }

  private static int CalculateYears(ref DateTime current, DateTime endDate)
  {
    int years = 0;
    while (current.AddYears(years + 1) <= endDate)
    {
      years++;
    }
    current = current.AddYears(years);
    return years;
  }

  private static int CalculateMonths(ref DateTime current, DateTime endDate)
  {
    int months = 0;
    while (current.AddMonths(months + 1) <= endDate)
    {
      months++;
    }
    current = current.AddMonths(months);
    return months;
  }

  public string GetDurationSummary()
  {
    if (Years > 0)
      return FormatDuration(Years, "year", Months, "month");
    if (Months > 0)
      return FormatDuration(Months, "month", WeeksInMonth > 0 ? WeeksInMonth : Days, WeeksInMonth > 0 ? "week" : "day");
    if (Days > 0)
      return FormatDuration(Days, "day", Hours, "hour");
    if (Hours > 0)
      return FormatDuration(Hours, "hour", Minutes, "minute");
    if (Minutes > 0)
      return FormatDuration(Minutes, "minute", Seconds, "second");
    return $"{Seconds} second{(Seconds == 1 ? "" : "s")}";
  }

  private static string FormatDuration(int primaryValue, string primaryUnit, int secondaryValue, string secondaryUnit)
  {
    return $"{primaryValue} {primaryUnit}{(primaryValue == 1 ? "" : "s")}, {secondaryValue} {secondaryUnit}{(secondaryValue == 1 ? "" : "s")}";
  }
}
