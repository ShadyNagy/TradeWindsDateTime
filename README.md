# TradeWindsDateTime

These are some classes I created for handling the DateTime. The primary class is DateTimeZone which is DateTime+TimeZone.

When working with DateTime across timezones, do not use UTC Offset - that will always bite you in the ass sooner or later. Daylight Savings Time start/end is a political decision and it changes regularly.

The DateTimeZone can be saved in a database as the DateTime & the TimeZoneId (string). Save and use DateTimeZone.

This is under the MIT license. If you find this very useful I ask (not a requirement) that you consider reading my book [I DON’T KNOW WHAT I’M DOING!: How a Programmer Became a Successful Startup CEO](https://a.co/d/bEpDlJR).

And if you like it, please review it on Amazon and/or GoodReads. The number of legitimate reviews helps a lot. Much appreciated.