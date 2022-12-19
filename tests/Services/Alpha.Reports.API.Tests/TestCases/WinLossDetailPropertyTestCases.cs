namespace Alpha.Reports.API.Tests.TestCases;

public static class WinLossDetailPropertyTestCases
{
    private static IEnumerable<object[]> allProperties = new List<object[]>()
    {
        new object[] { "CustId" },
        new object[] { "CustName" },
        new object[] { "Turnover" },
        new object[] { "BetCount" },
        new object[] { "MemberWinLoss" },
        new object[] { "MemberComm" },
        new object[] { "AgentWinLoss" },
        new object[] { "AgentComm" },
        new object[] { "MasterWinLoss" },
        new object[] { "MasterComm" },
        new object[] { "SuperWinLoss" },
        new object[] { "SuperComm" }
    };

    public static IEnumerable<object[]> AllProperties => allProperties;
}
