namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents user-specific data including personal information.
    /// </summary>
    public class UserData : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user data.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated user identity.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the associated user identity object.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the user data is locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }
    }
}
