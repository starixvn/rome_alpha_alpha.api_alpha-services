using System;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Services;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using Xunit;

namespace Alpha.Reports.API.Tests.Services;

public class ReportDateServiceTests
{
    [Fact]
    public void GetBalanceUpDay_ReturnsBalanceUpDayFromCache()
    {
        // Arrange
        var expectedDate = new DateTime(2024, 1, 1);
        var model = new BalanceUpDayAndMoveBetsDayModel(expectedDate, expectedDate.AddDays(1));
        var cacheService = Substitute.For<ICacheService>();
        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>()).Returns(model);
        var dynamicRepository = Substitute.For<IDynamicRepository>();
        var service = new ReportDateService(dynamicRepository, cacheService);

        // Act
        var result = service.GetBalanceUpDay();

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void GetMoveBetsDay_ReturnsMoveBetsDayFromCache()
    {
        // Arrange
        var expectedDate = new DateTime(2024, 1, 2);
        var model = new BalanceUpDayAndMoveBetsDayModel(expectedDate.AddDays(-1), expectedDate);
        var cacheService = Substitute.For<ICacheService>();
        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>()).Returns(model);
        var dynamicRepository = Substitute.For<IDynamicRepository>();
        var service = new ReportDateService(dynamicRepository, cacheService);

        // Act
        var result = service.GetMoveBetsDay();

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void GetBalanceUpDayAndMoveBetsDay_ReturnsModelFromCache()
    {
        // Arrange
        var model = new BalanceUpDayAndMoveBetsDayModel(DateTime.Now, DateTime.Now.AddDays(1));
        var cacheService = Substitute.For<ICacheService>();
        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>()).Returns(model);
        var dynamicRepository = Substitute.For<IDynamicRepository>();
        var service = new ReportDateService(dynamicRepository, cacheService);

        // Act
        var result = service.GetBalanceUpDayAndMoveBetsDay();

        // Assert
        Assert.Equal(model, result);
    }
}
