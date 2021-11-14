using System;

namespace BudgetManager.FinanceDataMining.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class DataColumnDescriptionAttribute : Attribute
    {
        public DataColumnDescriptionAttribute(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));
            this.Description = description;
        }

        public string Description { get; }
    }
}
