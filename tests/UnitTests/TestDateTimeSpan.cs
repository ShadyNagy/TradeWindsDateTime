
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

using TradeWindsDateTime;
using TradeWindsDateTime.Interfaces;

namespace UnitTests;

/// <summary>
/// Test DateTimeSpan
/// </summary>
public class TestDateTimeSpan
{
  [Fact]
  public void TestAll()
  {

    var dateTimeMarch = new DateTime(2024, 3, 15);
    var dateTimeSept = new DateTime(2024, 9, 15);
    IDateTimeSpan diff = DateTimeSpan.CalculateDifference(dateTimeMarch, dateTimeSept);
    Assert.Equal(0, diff.Years);
    Assert.Equal(6, diff.Months);
    Assert.Equal(0, diff.Days);
    Assert.Equal(0, diff.Hours);
    Assert.Equal(0, diff.Minutes);
    Assert.Equal(0, diff.Seconds);
    Assert.Equal(0, diff.WeeksInMonth);
    Assert.Equal(0, diff.DaysRemainderWeeks);
    Assert.Equal("6 months, 0 days", diff.GetDurationSummary());

    var dateTimeMarch2 = new DateTime(2024, 3, 10, 14, 30, 45);
    var dateTimeSept2 = new DateTime(2023, 9, 15, 12, 25, 12);
    IDateTimeSpan diff2 = DateTimeSpan.CalculateDifference(dateTimeMarch2, dateTimeSept2);
    Assert.Equal(0, diff2.Years);
    Assert.Equal(5, diff2.Months);
    Assert.Equal(24, diff2.Days);
    Assert.Equal(2, diff2.Hours);
    Assert.Equal(5, diff2.Minutes);
    Assert.Equal(33, diff2.Seconds);
    Assert.Equal(3, diff2.WeeksInMonth);
    Assert.Equal(3, diff2.DaysRemainderWeeks);
    Assert.Equal("5 months, 3 weeks", diff2.GetDurationSummary());

    var dateTimeMarch3 = new DateTime(2024, 3, 10, 14, 30, 45);
    var dateTimeSept3 = new DateTime(2021, 9, 15, 12, 25, 12);
    IDateTimeSpan diff3 = DateTimeSpan.CalculateDifference(dateTimeMarch3, dateTimeSept3);
    Assert.Equal(2, diff3.Years);
    Assert.Equal(5, diff3.Months);
    Assert.Equal(24, diff3.Days);
    Assert.Equal(3, diff3.WeeksInMonth);
    Assert.Equal(3, diff3.DaysRemainderWeeks);
    Assert.Equal("2 years, 5 months", diff3.GetDurationSummary());

    var dateTimeMarch4 = new DateTime(2024, 3, 10, 14, 30, 45);
    var dateTimeSept4 = new DateTime(2024, 3, 10, 12, 35, 12);
    IDateTimeSpan diff4 = DateTimeSpan.CalculateDifference(dateTimeMarch4, dateTimeSept4);
    Assert.Equal(0, diff4.Years);
    Assert.Equal(0, diff4.Months);
    Assert.Equal(0, diff4.Days);
    Assert.Equal(1, diff4.Hours);
    Assert.Equal(55, diff4.Minutes);
    Assert.Equal(33, diff4.Seconds);
    Assert.Equal(0, diff4.WeeksInMonth);
    Assert.Equal(0, diff4.DaysRemainderWeeks);
    Assert.Equal("1 hour, 55 minutes", diff4.GetDurationSummary());
  }
}
