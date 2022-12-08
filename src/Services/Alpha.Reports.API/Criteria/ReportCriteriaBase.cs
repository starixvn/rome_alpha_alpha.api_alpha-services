
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public abstract class ReportCriteriaBase : CriteriaBase
{
    protected ReportCriteriaBase(
        int custId,
        DateTime fromDate,
        DateTime toDate,
        DateTime balanceUpDay,
        DateTime moveBetsDay)
    {
        CustId = custId;
        FromDate = fromDate.Date;
        ToDate = toDate.Date;
        BalanceUpDay = balanceUpDay;
        MoveBetsDay = moveBetsDay;
    }

    public int CustId { get; }

    public DateTime FromDate { get; }

    public DateTime ToDate { get; }

    public DateTime BalanceUpDay { get; }

    public DateTime MoveBetsDay { get; }

    public override bool IsValid() => CustId > 0 && FromDate <= ToDate;

    public bool IsNotValid() => !IsValid();
}