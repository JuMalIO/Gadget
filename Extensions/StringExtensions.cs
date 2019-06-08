using System;
using System.Globalization;

namespace Gadget.Extensions
{
    public static class StringExtensions
    {
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
    }
}
