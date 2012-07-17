using System;
using System.Globalization;

namespace Finkit.ManicTime.Server.SampleClient
{
    public static class DateTimeExtensions
    {
        public static string FormatIso8601(this DateTimeOffset time)
        {
            return time.ToString("o", CultureInfo.InvariantCulture);
        }

        public static string FormatIso8601(this DateTime time)
        {
            return time.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
