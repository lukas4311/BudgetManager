using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents the identity information of a user including login credentials and associated data.
    /// </summary>
    public class UserIdentity : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user identity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the login username of the user.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the hashed password of the user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the user-specific data associated with this identity.
        /// </summary>
        public UserData UserData { get; set; }

        /// <summary>
        /// Gets or sets the list of bank accounts associated with this user identity.
        /// </summary>
        public List<BankAccount> BankAccounts { get; set; }

        /// <summary>
        /// Gets or sets the list of crypto trade histories associated with this user identity.
        /// </summary>
        public List<CryptoTradeHistory> CryptoTradesHistory { get; set; }

        /// <summary>
        /// Gets or sets the list of other investments associated with this user identity.
        /// </summary>
        public List<OtherInvestment> OtherInvestments { get; set; }

        /// <summary>
        /// Gets or sets the list of commodity trade histories associated with this user identity.
        /// </summary>
        public List<ComodityTradeHistory> ComodityTradeHistory { get; set; }

        /// <summary>
        /// Gets or sets the list of broker reports to process associated with this user identity.
        /// </summary>
        public List<BrokerReportToProcess> BrokerReportsToProcess { get; set; }

        /// <summary>
        /// Gets or sets the list of notifications associated with this user identity.
        /// </summary>
        public List<Notification> Notifications { get; set; }
    }
}
