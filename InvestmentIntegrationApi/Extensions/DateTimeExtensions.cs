using System;

namespace FinanceDataMining.Extensions
{
    internal static class DateTimeExtensions
    {
        public static double ConvertToUnixTimestamp(this DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dateTime.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
