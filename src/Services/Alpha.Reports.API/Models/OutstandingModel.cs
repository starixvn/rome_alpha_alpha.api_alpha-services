using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Models;

public class OutstandingModel
{
    public OutstandingModel()
    {
    }

    public OutstandingModel(
        int custId,
        string custName,
        decimal memberOutstanding,
        decimal agentOutstanding,
        decimal masterOutstanding,
        decimal superOutstanding)
    {
        CustId = custId;
        CustName = custName;
        MemberOutstanding = memberOutstanding;
        AgentOutstanding = agentOutstanding;
        MasterOutstanding = masterOutstanding;
        SuperOutstanding = superOutstanding;
    }

    public int CustId { get; set; }

    public string CustName { get; set; }

    public decimal MemberOutstanding { get; private set; }

    public decimal AgentOutstanding { get; private set; }

    public decimal MasterOutstanding { get; private set; }

    public decimal SuperOutstanding { get; private set; }

    public static OutstandingModel GetAccumulatedData(IEnumerable<OutstandingModel> outstandings)
    {
        var accumulatedData = new OutstandingModel();

        foreach (var outstanding in outstandings)
        {
            accumulatedData.MemberOutstanding += outstanding.MemberOutstanding;
            accumulatedData.AgentOutstanding += outstanding.AgentOutstanding;
            accumulatedData.MasterOutstanding += outstanding.MasterOutstanding;
            accumulatedData.SuperOutstanding += outstanding.SuperOutstanding;
        }

        return accumulatedData;
    }

    public decimal GetValueByLevel(byte level)
        => level switch
        {
            UserLevelModel.Super => SuperOutstanding,
            UserLevelModel.Master => MasterOutstanding,
            UserLevelModel.Agent => AgentOutstanding,
            UserLevelModel.Member => MemberOutstanding,
            _ => MemberOutstanding,
        };
}