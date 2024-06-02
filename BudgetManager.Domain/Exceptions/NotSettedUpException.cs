using System;

namespace BudgetManager.Domain.Exceptions;

/// <summary>
/// Exception for service was not setup
/// </summary>
public class NotSettedUpException : Exception
{
    /// <summary>
    /// Create instance of exception
    /// </summary>
    public NotSettedUpException() : base()
    {
    }

    /// <summary>
    /// Create instance of exception
    /// </summary>
    /// <param name="message">Exception message</param>
    public NotSettedUpException(string message) : base(message)
    {
    }

    /// <summary>
    /// Create instance of exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exceptino</param>
    public NotSettedUpException(string message, Exception innerException) : base(message, innerException)
    {
    }
}