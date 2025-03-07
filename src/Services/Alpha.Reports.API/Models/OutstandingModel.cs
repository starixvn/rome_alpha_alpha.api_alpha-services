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
        var accumulatedData = new OutstandingModel
        {
            ToWin = outstandings.Select(i => i.ToWin).Sum(),
            AgentRisk = outstandings.Select(i => i.AgentRisk).Sum(),
            MasterRisk = outstandings.Select(i => i.MasterRisk).Sum(),
            SuperRisk = outstandings.Select(i => i.SuperRisk).Sum()
        };

        foreach (var outstanding in outstandings)
        {
            accumulatedData.MemberOutstanding += outstanding.MemberOutstanding;
            accumulatedData.AgentOutstanding += outstanding.AgentOutstanding;
            accumulatedData.MasterOutstanding += outstanding.MasterOutstanding;
            accumulatedData.SuperOutstanding += outstanding.SuperOutstanding;
        }

        return accumulatedData;
    }
}