using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public abstract class ReportCriteriaBase : CriteriaBase
{
    protected ReportCriteriaBase(
        int custId,
        DateTime balanceUpDay,
        DateTime moveBetsDay)
    {
        CustId = custId;
        BalanceUpDay = balanceUpDay;
        MoveBetsDay = moveBetsDay;
    }

    public int CustId { get; }

    public DateTime BalanceUpDay { get; }

    public DateTime MoveBetsDay { get; }

    public override bool IsValid() => CustId > 0;

    public bool IsNotValid() => !IsValid();
}