using System;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a notification model.
/// </summary>
public class NotificationModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the notification (nullable).
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the user identity ID associated with this notification.
    /// </summary>
    public int UserIdentityId { get; set; }

    /// <summary>
    /// Gets or sets the heading of the notification.
    /// </summary>
    public string Heading { get; set; }

    /// <summary>
    /// Gets or sets the content of the notification.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the notification is displayed.
    /// </summary>
    public bool IsDisplayed { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the notification was created.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
