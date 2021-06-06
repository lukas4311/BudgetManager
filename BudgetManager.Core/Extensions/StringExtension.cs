using BudgetManager.Core.Exceptions;
using System;

namespace BudgetManager.Core.Extensions
{
    public static class StringExtension
    {
        public static DateTime ParseToUtcDateTime(this string unixTimeSeconds)
        {
            DateTime convertedUnixSeconds = ConvertUnixTimeSeconds(unixTimeSeconds);
            return DateTime.SpecifyKind(convertedUnixSeconds, DateTimeKind.Utc); ;
        }
        public static DateTime ParseToLocalDateTime(this string unixTimeSeconds)
        {
            return ConvertUnixTimeSeconds(unixTimeSeconds);
        }

        private static DateTime ConvertUnixTimeSeconds(string unixTimeSeconds)
        {
            if (!long.TryParse(unixTimeSeconds, out long unixTimeSecondsParsed))
                throw new BadUnixTimeFormat($"Format of uniix time seconds cannot be parsed. {unixTimeSeconds}");

            return DateTimeOffset.FromUnixTimeSeconds(unixTimeSecondsParsed).DateTime;
        }
    }
}
