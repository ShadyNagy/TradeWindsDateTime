using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWindsDateTime.Interfaces;

public interface IDateTimeSpan
{
  int Years { get; }
  int Months { get; }
  int Days { get; }
  int Hours { get; }
  int Minutes { get; }
  int Seconds { get; }
  int Milliseconds { get; }
  int WeeksInMonth { get; }
  int DaysRemainderWeeks { get; }
  string GetDurationSummary();
}
