using BudgetManager.Core.Exceptions;
using System;

namespace BudgetManager.Core.Extensions
{
    /// <summary>
/// Provides extension methods for string objects to parse Unix time seconds to DateTime.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Parses a Unix time seconds string to a UTC DateTime.
    /// </summary>
    /// <param name="unixTimeSeconds">The Unix time seconds string to parse.</param>
    /// <returns>A DateTime object representing the time in UTC.</returns>
    /// <exception cref="BadUnixTimeFormat">Thrown when the format of Unix time seconds cannot be parsed.</exception>
    public static DateTime ParseToUtcDateTime(this string unixTimeSeconds)
    {
        DateTime convertedUnixSeconds = ConvertUnixTimeSeconds(unixTimeSeconds);
        return DateTime.SpecifyKind(convertedUnixSeconds, DateTimeKind.Utc);
    }

    /// <summary>
    /// Parses a Unix time seconds string to a local DateTime.
    /// </summary>
    /// <param name="unixTimeSeconds">The Unix time seconds string to parse.</param>
    /// <returns>A DateTime object representing the time in the local time zone.</returns>
    /// <exception cref="BadUnixTimeFormat">Thrown when the format of Unix time seconds cannot be parsed.</exception>
    public static DateTime ParseToLocalDateTime(this string unixTimeSeconds)
    {
        return ConvertUnixTimeSeconds(unixTimeSeconds);
    }

    /// <summary>
    /// Converts a Unix time seconds string to a DateTime object.
    /// </summary>
    /// <param name="unixTimeSeconds">The Unix time seconds string to convert.</param>
    /// <returns>A DateTime object representing the specified time.</returns>
    /// <exception cref="BadUnixTimeFormat">Thrown when the format of Unix time seconds cannot be parsed.</exception>
    private static DateTime ConvertUnixTimeSeconds(string unixTimeSeconds)
    {
        if (!long.TryParse(unixTimeSeconds, out long unixTimeSecondsParsed))
            throw new BadUnixTimeFormat($"Format of Unix time seconds cannot be parsed: {unixTimeSeconds}");

        return DateTimeOffset.FromUnixTimeSeconds(unixTimeSecondsParsed).DateTime;
    }
}
}
