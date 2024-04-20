using System;

namespace BudgetManager.Domain.DTOs;

public class NotificationModel : IDtoModel
{
    public int? Id { get; set; }

    public int UserIdentityId { get; set; }

    public string Heading { get; set; }

    public string Content { get; set; }

    public bool IsDisplayed { get; set; }

    public DateTime Timestamp { get; set; }
}