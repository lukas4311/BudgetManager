using System;

namespace BudgetManager.Data.DataModels;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Heading { get; set; }
    public string Content { get; set; }
    public bool IsDisplayed { get; set; }
    public DateTime Timestamp { get; set; }
    public string AttachmentUrl { get; set; }

    public UserIdentity User { get; set; }
}