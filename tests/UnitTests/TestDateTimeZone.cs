
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

namespace UnitTests;
public class TestDateTimeZone
{
  [Fact]
  public void TestAll()
  {
    var dateTimeZone = new DateTimeZone(new DateTime(1955, 9, 26, 1, 2, 3), "Mountain Standard Time");
    Assert.Equal(new DateTime(1955, 9, 26, 1, 2, 3), dateTimeZone.DateTime);
    Assert.Equal("Mountain Standard Time", dateTimeZone.TimeZoneId);

    var dateTimeZone2 = dateTimeZone.ConvertTimeZone("Eastern Standard Time");
    Assert.Equal(new DateTime(1955, 9, 26, 3, 2, 3), dateTimeZone2.DateTime);
    Assert.Equal("Eastern Standard Time", dateTimeZone2.TimeZoneId);

    // verify it maps both to UTC, then compares
    Assert.Equal(dateTimeZone, dateTimeZone2);

    var dateTimeZone3 = new DateTimeZone(new DateTime(1955, 9, 26, 1, 2, 4), "Mountain Standard Time");
    Assert.True(dateTimeZone < dateTimeZone3);
    Assert.True(dateTimeZone <= dateTimeZone3);
    Assert.False(dateTimeZone == dateTimeZone3);
    Assert.False(dateTimeZone > dateTimeZone3);
    Assert.False(dateTimeZone >= dateTimeZone3);

    Assert.True(dateTimeZone3 > dateTimeZone);
    Assert.True(dateTimeZone3 >= dateTimeZone);
    Assert.False(dateTimeZone3 == dateTimeZone);
    Assert.False(dateTimeZone3 < dateTimeZone);
    Assert.False(dateTimeZone3 <= dateTimeZone);

    var now = DateTime.Now;
    var utcNow = DateTime.UtcNow;

    var dateTimeUtcTimeZone = new DateTimeZone(utcNow, DateTimeZone.UtcTimeZone);
    var dateTimeAsUtc = dateTimeUtcTimeZone.UtcDateTime;
    Assert.Equal(utcNow, dateTimeAsUtc);

    // to hard to write legit tests against any timezone
    if (TimeZoneInfo.Local.Id != "Mountain Standard Time")
      return;

    var dateTimeLocal = new DateTimeZone();
    Assert.True(dateTimeLocal.DateTime - now < new TimeSpan(0, 0, 0, 2));
    Assert.Equal("Mountain Standard Time", dateTimeLocal.TimeZoneId);

    var dateTimeEast = DateTimeZone.Now("Eastern Standard Time");
    Assert.True(dateTimeEast.DateTime.AddHours(-2) - now < new TimeSpan(0, 0, 0, 2));
    Assert.Equal("Eastern Standard Time", dateTimeEast.TimeZoneId);

    var dateTimeUtc = dateTimeLocal.UtcDateTime;
    Assert.True(dateTimeUtc - utcNow < new TimeSpan(0, 0, 0, 2));
  }
}
