using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class UserIdentity : IDataModel
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public UserData UserData { get; set; }

        public List<BankAccount> BankAccounts { get; set; }

        public List<CryptoTradeHistory> CryptoTradesHistory { get; set; }

        public List<OtherInvestment> OtherInvestments { get; set; }

        public List<ComodityTradeHistory> ComodityTradeHistory { get; set; }

        public List<BrokerReportToProcess> BrokerReportsToProcess { get; set; }
    }
}
