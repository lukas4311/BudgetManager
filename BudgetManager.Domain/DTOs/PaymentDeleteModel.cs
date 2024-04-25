using System.Collections.Generic;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a model for deleting payment-related data.
/// </summary>
public record PaymentDeleteModel(
    BankAccount bankAccount,
    IEnumerable<InterestRate> interests,
    IEnumerable<Payment> payments,
    IEnumerable<PaymentTag> paymentTags);