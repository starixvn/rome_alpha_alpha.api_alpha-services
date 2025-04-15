using System.Data;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.DataAccesses;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Reports.API.Tests.Database.WinLossDetail;
using Alpha.Reports.API.Tests.Fixtures;
using KellermanSoftware.CompareNetObjects;
using NSubstitute;

namespace Alpha.Reports.API.Tests.DataAccesses;

[Collection("ObjectDbCollection")]
public class WinLossDetailDataAccessTests
{
    private readonly ObjectDbFixture objectDbFixture;
    private readonly WinLossDetailDataAccess dataAccess;
    private readonly CompareLogic compareLogic;

    public WinLossDetailDataAccessTests(ObjectDbFixture objectDbFixture)
    {
        this.objectDbFixture = objectDbFixture;
        dataAccess = new WinLossDetailDataAccess();
        compareLogic = new CompareLogic();
    }

    [Fact]
    public void GetSuperWinLossDetail_NullDataReturnFromDb_ReturnsDefaultData()
    {
        // Arrange
        var criteria = new GetWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now, 0);
        var expectedValue = new ReportDataModel<SuperWinLossDetailModel>(new List<SuperWinLossDetailModel>(), new SuperWinLossDetailModel());
        IDataReader dataReader = null;
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>())
            .Returns(dataReader);

        // Act
        var actualValue = dataAccess.GetSuperWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetSuperWinLossDetail_WhenReportHasData_ReturnsDataFromObjectDb()
    {
        // Arrange
        var criteria = new GetWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now, 0);
        var dataSet = WinLossDetailDatabase.MasterWinLoss.CreateDataReader();
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>()).Returns(dataSet);

        var winLossDetails = new List<SuperWinLossDetailModel>
        {
            new SuperWinLossDetailModel
            {
                CustId = 1,
                CustName = "Master1",
                BetCount = 11,
                Turnover = 0,
                MemberWinLoss = 3,
                MemberComm = 4,
                AgentWinLoss = 5,
                AgentComm = 6,
                MasterWinLoss = 7,
                MasterComm = 8,
                SuperWinLoss = 9,
                SuperComm = 10,
                GrossComm = 2,
            },
            new SuperWinLossDetailModel {
                CustId = 11,
                CustName = "Master2",
                BetCount = 111,
                Turnover = 10,
                MemberWinLoss = 13,
                MemberComm = 14,
                AgentWinLoss = 15,
                AgentComm = 16,
                MasterWinLoss = 17,
                MasterComm = 18,
                SuperWinLoss = 19,
                SuperComm = 110,
                GrossComm = 12,
            }
        };

        var total = new SuperWinLossDetailModel
        {
            CustId = 0,
            CustName = "Total",
            BetCount = 211,
            Turnover = 20,
            MemberWinLoss = 23,
            MemberComm = 24,
            AgentWinLoss = 25,
            AgentComm = 26,
            MasterWinLoss = 27,
            MasterComm = 28,
            SuperWinLoss = 29,
            SuperComm = 210,
            GrossComm = 22,
        };

        var expectedValue = new ReportDataModel<SuperWinLossDetailModel>(winLossDetails, total);

        // Act
        var actualValue = dataAccess.GetSuperWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetSuperWinLossDetail_WhenReportHasDataWithNoTotalTable_ReturnsDataFromObjectDb()
    {
        // Arrange
        var criteria = new GetWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now, 0);
        var dataSet = WinLossDetailDatabase.MasterWinLossWinNoTotalTable.CreateDataReader();
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>()).Returns(dataSet);

        var winLossDetails = new List<SuperWinLossDetailModel>
        {
            new SuperWinLossDetailModel
            {
                CustId = 1,
                CustName = "Master1",
                BetCount = 11,
                Turnover = 0,
                MemberWinLoss = 3,
                MemberComm = 4,
                AgentWinLoss = 5,
                AgentComm = 6,
                MasterWinLoss = 7,
                MasterComm = 8,
                SuperWinLoss = 9,
                SuperComm = 10,
                GrossComm = 2,
            },
            new SuperWinLossDetailModel {
                CustId = 11,
                CustName = "Master2",
                BetCount = 111,
                Turnover = 10,
                MemberWinLoss = 13,
                MemberComm = 14,
                AgentWinLoss = 15,
                AgentComm = 16,
                MasterWinLoss = 17,
                MasterComm = 18,
                SuperWinLoss = 19,
                SuperComm = 110,
                GrossComm = 12,
            }
        };

        var total = new SuperWinLossDetailModel();

        var expectedValue = new ReportDataModel<SuperWinLossDetailModel>(winLossDetails, total);

        // Act
        var actualValue = dataAccess.GetSuperWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetMasterWinLossDetail_NullDataReturnFromDb_ReturnsDefaultData()
    {
        // Arrange
        var criteria = new GetWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now, 0);
        var expectedValue = new ReportDataModel<MasterWinLossDetailModel>(new List<MasterWinLossDetailModel>(), new MasterWinLossDetailModel());
        IDataReader dataReader = null;
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>())
            .Returns(dataReader);

        // Act
        var actualValue = dataAccess.GetMasterWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetMasterWinLossDetail_WhenReportHasData_ReturnsDataFromObjectDb()
    {
        // Arrange
        var criteria = new GetWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now, 0);
        var dataSet = WinLossDetailDatabase.AgentWinLoss.CreateDataReader();
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>()).Returns(dataSet);

        var winLossDetails = new List<MasterWinLossDetailModel>
        {
            new MasterWinLossDetailModel
            {
                CustId = 1,
                CustName = "Agent1",
                BetCount = 11,
                Turnover = 0,
                MemberWinLoss = 3,
                MemberComm = 4,
                AgentWinLoss = 5,
                AgentComm = 6,
                MasterWinLoss = 7,
                MasterComm = 8,
                GrossComm = 2,
            },
            new MasterWinLossDetailModel {
                CustId = 11,
                CustName = "Agent2",
                BetCount = 111,
                Turnover = 10,
                MemberWinLoss = 13,
                MemberComm = 14,
                AgentWinLoss = 15,
                AgentComm = 16,
                MasterWinLoss = 17,
                MasterComm = 18,
                GrossComm = 12,
            }
        };

        var total = new MasterWinLossDetailModel
        {
            CustId = 0,
            CustName = "Total",
            BetCount = 211,
            Turnover = 20,
            MemberWinLoss = 23,
            MemberComm = 24,
            AgentWinLoss = 25,
            AgentComm = 26,
            MasterWinLoss = 27,
            MasterComm = 28,
            GrossComm = 22,
        };

        var expectedValue = new ReportDataModel<MasterWinLossDetailModel>(winLossDetails, total);

        // Act
        var actualValue = dataAccess.GetMasterWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetMasterWinLossDetail_WhenReportHasDataWithNoTotalTable_ReturnsDataFromObjectDb()
    {
        // Arrange
        var criteria = new GetWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now, 0);
        var dataSet = WinLossDetailDatabase.MasterWinLossWinNoTotalTable.CreateDataReader();
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>()).Returns(dataSet);

        var winLossDetails = new List<MasterWinLossDetailModel>
        {
            new MasterWinLossDetailModel
            {
                CustId = 1,
                CustName = "Master1",
                BetCount = 11,
                Turnover = 0,
                MemberWinLoss = 3,
                MemberComm = 4,
                AgentWinLoss = 5,
                AgentComm = 6,
                MasterWinLoss = 7,
                MasterComm = 8,
                GrossComm = 2,
            },
            new MasterWinLossDetailModel {
                CustId = 11,
                CustName = "Master2",
                BetCount = 111,
                Turnover = 10,
                MemberWinLoss = 13,
                MemberComm = 14,
                AgentWinLoss = 15,
                AgentComm = 16,
                MasterWinLoss = 17,
                MasterComm = 18,
                GrossComm = 12,
            }
        };

        var total = new MasterWinLossDetailModel();

        var expectedValue = new ReportDataModel<MasterWinLossDetailModel>(winLossDetails, total);

        // Act
        var actualValue = dataAccess.GetMasterWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetAgentWinLossDetail_NullDataReturnFromDb_ReturnsDefaultData()
    {
        // Arrange
        var criteria = new GetMemberWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now);
        var expectedValue = new ReportDataModel<AgentWinLossDetailModel>(new List<AgentWinLossDetailModel>(), new AgentWinLossDetailModel());
        IDataReader dataReader = null;
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>())
            .Returns(dataReader);

        // Act
        var actualValue = dataAccess.GetAgentWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetAgentWinLossDetail_WhenReportHasData_ReturnsDataFromObjectDb()
    {
        // Arrange
        var criteria = new GetMemberWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now);
        var dataSet = WinLossDetailDatabase.MemberWinLoss.CreateDataReader();
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>()).Returns(dataSet);

        var winLossDetails = new List<AgentWinLossDetailModel>
        {
            new AgentWinLossDetailModel
            {
                CustId = 1,
                CustName = "Member1",
                BetCount = 11,
                Turnover = 0,
                MemberWinLoss = 3,
                MemberComm = 4,
                AgentWinLoss = 5,
                AgentComm = 6,
                GrossComm = 2,
            },
            new AgentWinLossDetailModel {
                CustId = 11,
                CustName = "Member2",
                BetCount = 111,
                Turnover = 10,
                MemberWinLoss = 13,
                MemberComm = 14,
                AgentWinLoss = 15,
                AgentComm = 16,
                GrossComm = 12,
            }
        };

        var total = new AgentWinLossDetailModel
        {
            CustId = 0,
            CustName = "Total",
            BetCount = 211,
            Turnover = 20,
            MemberWinLoss = 23,
            MemberComm = 24,
            AgentWinLoss = 25,
            AgentComm = 26,
            GrossComm = 22,
        };

        var expectedValue = new ReportDataModel<AgentWinLossDetailModel>(winLossDetails, total);

        // Act
        var actualValue = dataAccess.GetAgentWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }

    [Fact]
    public void GetAgentWinLossDetail_WhenReportHasDataWithNoTotalTable_ReturnsDataFromObjectDb()
    {
        // Arrange
        var criteria = new GetMemberWinLossDetailCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>(), 0, string.Empty, DateTime.Now, DateTime.Now);
        var dataSet = WinLossDetailDatabase.MasterWinLossWinNoTotalTable.CreateDataReader();
        objectDbFixture.ObjectDb.ExecuteReaderAsync(Arg.Any<GetWinLossDetailCriteria>()).Returns(dataSet);

        var winLossDetails = new List<AgentWinLossDetailModel>
        {
            new AgentWinLossDetailModel
            {
                CustId = 1,
                CustName = "Master1",
                BetCount = 11,
                Turnover = 0,
                MemberWinLoss = 3,
                MemberComm = 4,
                AgentWinLoss = 5,
                AgentComm = 6,
                GrossComm = 2,
            },
            new AgentWinLossDetailModel {
                CustId = 11,
                CustName = "Master2",
                BetCount = 111,
                Turnover = 10,
                MemberWinLoss = 13,
                MemberComm = 14,
                AgentWinLoss = 15,
                AgentComm = 16,
                GrossComm = 12,
            }
        };

        var total = new AgentWinLossDetailModel();

        var expectedValue = new ReportDataModel<AgentWinLossDetailModel>(winLossDetails, total);

        // Act
        var actualValue = dataAccess.GetAgentWinLossDetail(criteria).GetAwaiter().GetResult();

        // Assert
        Assert.True(compareLogic.Compare(expectedValue, actualValue).AreEqual);
    }
}
