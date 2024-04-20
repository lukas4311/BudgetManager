using System;

namespace BudgetManager.Data.DataModels;

public class Notification : IDataModel
{
    public int Id { get; set; }

    public int UserIdentityId { get; set; }

    public string Heading { get; set; }

    public string Content { get; set; }

    public bool IsDisplayed { get; set; }

    public DateTime Timestamp { get; set; }

    public string AttachmentUrl { get; set; }

    public UserIdentity UserIdentity { get; set; }
}