using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public abstract class ReportCriteriaBase : CriteriaBase
{
    protected ReportCriteriaBase(
        int custId,
        DateTimeOffset balanceUpDay,
        DateTimeOffset moveBetsDay)
    {
        CustId = custId;
        BalanceUpDay = balanceUpDay;
        MoveBetsDay = moveBetsDay;
    }

    public int CustId { get; }

    public DateTimeOffset BalanceUpDay { get; }

    public DateTimeOffset MoveBetsDay { get; }

    public override bool IsValid() => CustId > 0;

    public bool IsNotValid() => !IsValid();
}