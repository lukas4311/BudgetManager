using FinanceDataMining.Enums;
using System.ComponentModel;

namespace FinanceDataMining.Extensions
{
    internal static class ExchangesExtension
    {
        public static string ToDescriptionString(this Exchanges val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
