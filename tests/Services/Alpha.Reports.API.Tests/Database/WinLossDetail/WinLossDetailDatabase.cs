using System.Data;
using Alpha.Reports.API.Tests.Helpers;

namespace Alpha.Reports.API.Tests.Database.WinLossDetail;

public static class WinLossDetailDatabase
{
    private readonly static DataSet masterWinLoss = FileHelper.LoadDataSet("\\Database\\WinLossDetail\\MasterWinLossDetailHasData.xml");
    private readonly static DataSet agentWinLoss = FileHelper.LoadDataSet("\\Database\\WinLossDetail\\AgentWinLossDetailHasData.xml");
    private readonly static DataSet memberWinLoss = FileHelper.LoadDataSet("\\Database\\WinLossDetail\\MemberWinLossDetailHasData.xml");

    private readonly static DataSet masterWinLossWinNoTotalTable = FileHelper.LoadDataSet("\\Database\\WinLossDetail\\MasterWinLossDetailHasData WinNoTotalTable.xml");

    public static DataSet MasterWinLoss => masterWinLoss;

    public static DataSet AgentWinLoss => agentWinLoss;

    public static DataSet MemberWinLoss => memberWinLoss;

    public static DataSet MasterWinLossWinNoTotalTable => masterWinLossWinNoTotalTable;
}
