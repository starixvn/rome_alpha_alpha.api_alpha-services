namespace Alpha.Reports.API.Models;

public class FollowBetAccountModel
{
    public FollowBetAccountModel() { }

    public FollowBetAccountModel(
        int custId,
        string username,
        int agentId,
        int dummyCustId,
        string dummyUsername)
    {
        CustId = custId;
        Username = username;
        AgentId = agentId;
        DummyCustId = dummyCustId;
        DummyUsername = dummyUsername;
    }

    public int CustId { get; }

    public string Username { get; }

    public int AgentId { get; }

    public int DummyCustId { get; }

    public string DummyUsername { get; }
}