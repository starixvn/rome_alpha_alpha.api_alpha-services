using System;
using System.Collections.Generic;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Params;
using Xunit;

namespace Alpha.BetLists.API.Tests.Params;

public class GetWinLossBetListParamTests
{
    [Fact]
    public void Constructor_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var custId = 123;
        var roleId = 2;
        var fromDate = "2023-01-01";
        var toDate = "2023-01-31";
        var userName = "testuser";
        var siteName = "testsite";
        var productIds = new List<int> { 1, 2, 3 };
        var viewerId = 456;
        var settings = new BetListsAppSettingsModel();

        // Act
        var param = new GetWinLossBetListParam(
            custId, roleId, fromDate, toDate, userName, siteName, productIds, viewerId, settings);

        // Assert
        Assert.Equal(custId, param.CustId);
        Assert.Equal(roleId, param.RoleId);
        Assert.Equal(DateTime.Parse(fromDate), param.FromDate);
        Assert.Equal(DateTime.Parse(toDate), param.ToDate);
        Assert.Equal(userName, param.UserName);
        Assert.Equal(siteName, param.SiteName);
        Assert.Equal(productIds, param.ProductIds);
        Assert.Equal(viewerId, param.ViewerId);
        Assert.Equal(settings, param.Settings);
        Assert.Equal(10000, param.MaxTotalRow); // Default value
    }

    [Theory]
    [InlineData("2023-01-01", "2023-01-01")]
    [InlineData("2023-12-31", "2023-12-31")]
    [InlineData("invalid-date", "0001-01-01")] // DateTime.MinValue
    [InlineData("", "0001-01-01")] // DateTime.MinValue
    [InlineData(null, "0001-01-01")] // DateTime.MinValue
    public void Constructor_ParsesDatesCorrectly(string inputDate, string expectedDateString)
    {
        // Arrange
        var settings = new BetListsAppSettingsModel();
        var expectedDate = DateTime.Parse(expectedDateString);

        // Act
        var param = new GetWinLossBetListParam(
            1, 1, inputDate, inputDate, "user", "site", new List<int>(), 1, settings);

        // Assert
        Assert.Equal(expectedDate, param.FromDate);
        Assert.Equal(expectedDate, param.ToDate);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsValid_ReturnsExpectedResult(int custId, bool expectedValid)
    {
        // Arrange
        var settings = new BetListsAppSettingsModel();
        var param = new GetWinLossBetListParam(
            custId, 1, "2023-01-01", "2023-01-31", "user", "site", new List<int>(), 1, settings);

        // Act
        var isValid = param.IsValid();

        // Assert
        Assert.Equal(expectedValid, isValid);
    }

    [Fact]
    public void MaxTotalRow_CanBeSet()
    {
        // Arrange
        var settings = new BetListsAppSettingsModel();
        var param = new GetWinLossBetListParam(
            1, 1, "2023-01-01", "2023-01-31", "user", "site", new List<int>(), 1, settings);

        // Act
        param.MaxTotalRow = 5000;

        // Assert
        Assert.Equal(5000, param.MaxTotalRow);
    }

    [Fact]
    public void Constructor_HandlesEmptyProductIds()
    {
        // Arrange
        var settings = new BetListsAppSettingsModel();
        var emptyProductIds = new List<int>();

        // Act
        var param = new GetWinLossBetListParam(
            1, 1, "2023-01-01", "2023-01-31", "user", "site", emptyProductIds, 1, settings);

        // Assert
        Assert.NotNull(param.ProductIds);
        Assert.Empty(param.ProductIds);
    }
}
