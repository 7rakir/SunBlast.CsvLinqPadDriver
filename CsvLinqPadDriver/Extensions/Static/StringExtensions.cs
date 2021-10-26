using System;

namespace CsvLinqPadDriver.Extensions.Static
{
    public static class StringExtensions
    {
        public static int ToInt32(this string input) => int.Parse(input);

        public static double ToDouble(this string input) => double.Parse(input);

        public static int? ToNullableInt32(this string input) => int.TryParse(input, out var i) ? i : null;

        public static double? ToNullableDouble(this string input) => double.TryParse(input, out var i) ? i : null;

        public static bool ToBool(this string input) => bool.Parse(input);

        public static bool? ToNullableBool(this string input) => bool.TryParse(input, out var i) ? i : null;

        public static DateTime ToDateTime(this string input) => DateTime.Parse(input);

        public static DateTime? ToNullableDateTime(this string input) => DateTime.TryParse(input, out var i) ? i : null;
    }
}