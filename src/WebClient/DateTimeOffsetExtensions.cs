using System;
using System.Globalization;

namespace Finkit.ManicTime.WebClient
{
    public static class DateTimeOffsetExtensions
    {
        public static string FormatIso8601(this DateTimeOffset time)
        {
            return time.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
