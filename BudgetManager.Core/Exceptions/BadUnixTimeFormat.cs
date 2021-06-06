using System;

namespace BudgetManager.Core.Exceptions
{
    public class BadUnixTimeFormat : Exception
    {
        public BadUnixTimeFormat() : base()
        {
        }

        public BadUnixTimeFormat(string message) : base(message)
        {
        }

        public BadUnixTimeFormat(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
