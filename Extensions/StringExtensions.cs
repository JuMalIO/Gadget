using System;
using System.Globalization;

namespace Gadget.Extensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string str, string split)
        {
            return str.Split(new string[] { split }, StringSplitOptions.None);
        }

        public static string CutAfterText(this string str, string text)
        {
            var index = str.IndexOf(text);
            if (index >= 0)
            {
                return str.Substring(index + text.Length, str.Length - index - text.Length);
            }
            return str;
        }

        public static string CutBeforeText(this string str, string text)
        {
            var index = str.IndexOf(text);
            if (index >= 0)
            {
                return str.Substring(0, index);
            }
            return null;
        }

        public static DateTime ParseTime(this string str)
        {
            DateTime.TryParseExact(str, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time);
            return time;
        }

        public static DateTime ParseTimestamp(this string str)
        {
            long.TryParse(str, out var timespan);
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timespan).ToLocalTime();
            return date;
        }
    }
}
