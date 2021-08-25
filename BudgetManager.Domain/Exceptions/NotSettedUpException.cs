using System;

namespace BudgetManager.Domain.Exceptions
{
    public class NotSettedUpException : Exception
    {
        public NotSettedUpException() : base()
        {
        }

        public NotSettedUpException(string message) : base(message)
        {
        }

        public NotSettedUpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
