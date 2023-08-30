using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Utilities.Extensions;
using Alpha.Reports.API.Tests.TestCases;

namespace Alpha.Reports.API.Tests.Extensions;

public class EnumerableExtensionsTests
{
    private readonly List<SuperWinLossDetailModel> winLossDetails;

    private readonly List<SuperWinLossDetailModel> descendingSortedListByWinLoss;

    private readonly List<SuperWinLossDetailModel> ascendingSortedListByWinLoss;

    public EnumerableExtensionsTests()
    {
        winLossDetails = new List<SuperWinLossDetailModel>();
        descendingSortedListByWinLoss = new List<SuperWinLossDetailModel>();
        ascendingSortedListByWinLoss = new List<SuperWinLossDetailModel>();

        var numberOfRecords = 3;
        for (var i = 1; i <= numberOfRecords; i++)
        {
            winLossDetails.Add(new SuperWinLossDetailModel
            {
                CustId = i,
                CustName = $"Cust Name {i}",
                Turnover = i,
                BetCount = i,
                MemberWinLoss = i,
                MemberComm = i,
                AgentWinLoss = i,
                AgentComm = i,
                MasterWinLoss = i,
                MasterComm = i,
                SuperWinLoss = i,
                SuperComm = i
            });

            ascendingSortedListByWinLoss.Add(new SuperWinLossDetailModel
            {
                CustId = i,
                CustName = $"Cust Name {i}",
                Turnover = i,
                BetCount = i,
                MemberWinLoss = i,
                MemberComm = i,
                AgentWinLoss = i,
                AgentComm = i,
                MasterWinLoss = i,
                MasterComm = i,
                SuperWinLoss = i,
                SuperComm = i
            });

            var descendingValue = numberOfRecords - i + 1;
            descendingSortedListByWinLoss.Add(new SuperWinLossDetailModel
            {
                CustId = descendingValue,
                CustName = $"Cust Name {descendingValue}",
                Turnover = descendingValue,
                BetCount = descendingValue,
                MemberWinLoss = descendingValue,
                MemberComm = descendingValue,
                AgentWinLoss = descendingValue,
                AgentComm = descendingValue,
                MasterWinLoss = descendingValue,
                MasterComm = descendingValue,
                SuperWinLoss = descendingValue,
                SuperComm = descendingValue
            });
        }
    }

    [Theory]
    [MemberData(nameof(WinLossDetailPropertyTestCases.AllProperties), MemberType = typeof(WinLossDetailPropertyTestCases))]
    public void OrderBy_OrderByDescending_ReturnsDescendingOrder(string propertyName)
    {
        // Arrange
        var expectedList = descendingSortedListByWinLoss;

        // Act
        var actualList = winLossDetails.OrderBy(propertyName).ToList();

        // Assert
        AssertExtension.Equal(expectedList, actualList);
    }

    [Fact]
    public void OrderBy_OrderByDescendingCompany_ReturnsDescendingOrder()
    {
        // Arrange
        var expectedList = ascendingSortedListByWinLoss;

        // Act
        var actualList = winLossDetails.OrderBy("Company").ToList();

        // Assert
        AssertExtension.Equal(expectedList, actualList);
    }

    [Theory]
    [MemberData("AllProperties", MemberType = typeof(WinLossDetailPropertyTestCases))]
    public void OrderBy_OrderByAscending_ReturnsAscendingOrder(string propertyName)
    {
        // Arrange
        var expectedList = ascendingSortedListByWinLoss;

        // Act
        var actualList = winLossDetails.OrderBy(propertyName, descending: false).ToList();

        // Assert
        AssertExtension.Equal(expectedList, actualList);
    }

    [Theory]
    [InlineData("Company")]
    public void OrderBy_OrderByAscendingCompany_ReturnsAscendingOrder(string propertyName)
    {
        // Arrange
        var expectedList = descendingSortedListByWinLoss;

        // Act
        var actualList = winLossDetails.OrderBy(propertyName, descending: false).ToList();

        // Assert
        AssertExtension.Equal(expectedList, actualList);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("WrongPropertyName")]
    public void OrderBy_OrderByWrongPropertyName_ReturnsAscendingOrder(string propertyName)
    {
        // Arrange
        var expectedList = winLossDetails;

        // Act
        var actualList = winLossDetails.OrderBy(propertyName, descending: false).ToList();

        // Assert
        AssertExtension.Equal(expectedList, actualList);
    }

    [Theory]
    [InlineData(null)]
    public void OrderBy_OrderByNullPropertyName_ThrowsNullReferenceException(string propertyName)
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => winLossDetails.OrderBy(propertyName, descending: false).ToList());
    }

    [Fact]
    public void JoinWithComma_Always_ReturnsStringResult()
    {
        // Arrange
        var l = new List<int> { 1, 2 };
        var expectedValue = "1,2";

        // Act
        var actualValue = l.JoinWithComma();

        // Act & Assert
        Assert.Equal(expectedValue, actualValue);
    }
}
