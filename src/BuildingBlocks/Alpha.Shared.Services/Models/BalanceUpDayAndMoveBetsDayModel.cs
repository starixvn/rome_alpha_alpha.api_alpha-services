namespace Alpha.Shared.Services.Models;

/// <summary>
/// Shared model for balance up day and move bets day information
/// Used across Alpha.Reports.API and Alpha.BetLists.API
/// </summary>
public record BalanceUpDayAndMoveBetsDayModel(DateTime BalanceUpDay, DateTime MoveBetsDay);
