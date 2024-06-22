using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a notification sent to a user.
    /// </summary>
    public class Notification : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the notification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user to whom the notification belongs.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the heading or title of the notification.
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// Gets or sets the content or message body of the notification.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the notification has been displayed to the user.
        /// </summary>
        public bool IsDisplayed { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the notification was created or sent.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the URL of any attachment associated with the notification.
        /// </summary>
        public string AttachmentUrl { get; set; }

        /// <summary>
        /// Gets or sets the user entity to whom the notification belongs.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }
    }
}
