using System;

namespace BudgetManager.Domain.Exceptions
{
    public class InvalidToken : Exception
    {
        public InvalidToken() : base()
        {
        }

        public InvalidToken(string message) : base(message)
        {
        }

        public InvalidToken(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
