using System.Collections.Generic;
using Alpha.BetLists.API.Params;
using Xunit;

namespace Alpha.BetLists.API.Tests.Params;

public class GetRunningBetListParamTests
{
    [Fact]
    public void Constructor_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var custId = 123;
        var userName = "testuser";
        var siteName = "testsite";
        var productIds = new List<int> { 1, 2, 3 };

        // Act
        var param = new GetRunningBetListParam(custId, userName, siteName, productIds);

        // Assert
        Assert.Equal(custId, param.CustId);
        Assert.Equal(userName, param.UserName);
        Assert.Equal(siteName, param.SiteName);
        Assert.Equal(productIds, param.ProductIds);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsValid_ReturnsExpectedResult(int custId, bool expectedValid)
    {
        // Arrange
        var param = new GetRunningBetListParam(custId, "user", "site", new List<int>());

        // Act
        var isValid = param.IsValid();

        // Assert
        Assert.Equal(expectedValid, isValid);
    }

    [Fact]
    public void Constructor_HandlesEmptyProductIds()
    {
        // Arrange
        var emptyProductIds = new List<int>();

        // Act
        var param = new GetRunningBetListParam(1, "user", "site", emptyProductIds);

        // Assert
        Assert.NotNull(param.ProductIds);
        Assert.Empty(param.ProductIds);
    }

    [Fact]
    public void Constructor_HandlesNullValues()
    {
        // Arrange & Act
        var param = new GetRunningBetListParam(1, null, null, null);

        // Assert
        Assert.Equal(1, param.CustId);
        Assert.Null(param.UserName);
        Assert.Null(param.SiteName);
        Assert.Null(param.ProductIds);
    }
}
