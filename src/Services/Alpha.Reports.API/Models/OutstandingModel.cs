using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Models;

public class OutstandingModel
{
    public int CustId { get; set; }

    public string CustName { get; set; }

    public decimal MemberOutstanding { get; private set; }

    public decimal AgentOutstanding { get; private set; }

    public decimal MasterOutstanding { get; private set; }

    public decimal SuperOutstanding { get; private set; }

    public decimal? ToWin { get; set; }

    public decimal? SuperRisk { get; set; }

    public decimal? MasterRisk { get; set; }

    public decimal? AgentRisk { get; set; }
    public static OutstandingModel GetAccumulatedData(IEnumerable<OutstandingModel> outstandings)
    {
        var accumulatedData = new OutstandingModel();

        foreach (var outstanding in outstandings)
        {
            accumulatedData.ToWin = (accumulatedData.ToWin ?? 0) + (outstanding.ToWin ?? 0);
            accumulatedData.AgentRisk = (accumulatedData.AgentRisk ?? 0) + (outstanding.AgentRisk ?? 0);
            accumulatedData.MasterRisk = (accumulatedData.MasterRisk ?? 0) + (outstanding.MasterRisk ?? 0);
            accumulatedData.SuperRisk = (accumulatedData.SuperRisk ?? 0) + (outstanding.SuperRisk ?? 0);
            accumulatedData.MemberOutstanding += outstanding.MemberOutstanding;
            accumulatedData.AgentOutstanding += outstanding.AgentOutstanding;
            accumulatedData.MasterOutstanding += outstanding.MasterOutstanding;
            accumulatedData.SuperOutstanding += outstanding.SuperOutstanding;
        }

        return accumulatedData;
    }
}