using System;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public static class StringHelper
    {
        public static string[] SplitIntoLines(this string value, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return value.Split(new[] { "\r\n", "\r", "\n" }, stringSplitOptions);
        }
    }
}
