using System.Collections.Generic;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a summary of other investment balance data.
/// </summary>
public record OtherInvestmentBalanceSummaryModel(
    IEnumerable<OtherInvestmentBalaceHistoryModel> ActualBalanceData,
    IEnumerable<OtherInvestmentBalaceHistoryModel> OneYearEarlierBalanceData);