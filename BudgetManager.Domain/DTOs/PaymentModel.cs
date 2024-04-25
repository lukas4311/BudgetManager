using System;
using System.Collections.Generic;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a payment entity.
/// </summary>
public class PaymentModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the payment.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the amount of the payment.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the name associated with the payment.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the payment.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the date of the payment.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated bank account.
    /// </summary>
    public int? BankAccountId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the payment type.
    /// </summary>
    public int? PaymentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the payment category.
    /// </summary>
    public int? PaymentCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the payment type.
    /// </summary>
    public string PaymentTypeCode { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the payment category.
    /// </summary>
    public string PaymentCategoryIcon { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the payment category.
    /// </summary>
    public string PaymentCategoryCode { get; set; }

    /// <summary>
    /// Gets or sets the list of tags associated with the payment.
    /// </summary>
    public List<string> Tags { get; set; }
}