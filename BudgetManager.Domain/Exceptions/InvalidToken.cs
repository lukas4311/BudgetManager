using System;

namespace BudgetManager.Domain.Exceptions;

/// <summary>
/// Exception for invalid token
/// </summary>
public class InvalidToken : Exception
{
    /// <summary>
    /// Create instance of exception
    /// </summary>
    public InvalidToken() : base()
    {
    }

    /// <summary>
    /// Create instance of exception
    /// </summary>
    /// <param name="message">Exception message</param>
    public InvalidToken(string message) : base(message)
    {
    }

    /// <summary>
    /// Create instance of exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exceptino</param>
    public InvalidToken(string message, Exception innerException) : base(message, innerException)
    {
    }
}