using Alpha.Shared.Services.Models;
using Alpha.Shared.Services.Services;
using Alpha.Shared.Services.Criteria;
using Fanex.Data.Repository;
using Fanex.Caching;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class ReportDateServiceTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly ReportDateService service;

    public ReportDateServiceTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        cacheService = Substitute.For<ICacheService>();
        service = new ReportDateService(dynamicRepository, cacheService, "Alpha.BetLists");
    }

    [Fact]
    public void GetBalanceUpDay_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new DateTime(2023, 1, 15);
        var reportDateModel = new BalanceUpDayAndMoveBetsDayModel(expectedDate, new DateTime(2023, 1, 16));

        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>())
            .Returns(callInfo => callInfo.Arg<Func<BalanceUpDayAndMoveBetsDayModel>>()());

        dynamicRepository.Get<BalanceUpDayAndMoveBetsDayModel>(Arg.Any<GetBalanceUpDayAndMoveBetsDayCriteria>())
            .Returns(reportDateModel);

        // Act
        var result = service.GetBalanceUpDay();

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void GetMoveBetsDay_ReturnsCorrectDate()
    {
        // Arrange
        var expectedDate = new DateTime(2023, 1, 16);
        var reportDateModel = new BalanceUpDayAndMoveBetsDayModel(new DateTime(2023, 1, 15), expectedDate);

        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>())
            .Returns(callInfo => callInfo.Arg<Func<BalanceUpDayAndMoveBetsDayModel>>()());

        dynamicRepository.Get<BalanceUpDayAndMoveBetsDayModel>(Arg.Any<GetBalanceUpDayAndMoveBetsDayCriteria>())
            .Returns(reportDateModel);

        // Act
        var result = service.GetMoveBetsDay();

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void GetBalanceUpDayAndMoveBetsDay_CachesDataCorrectly()
    {
        // Arrange
        var reportDateModel = new BalanceUpDayAndMoveBetsDayModel(new DateTime(2023, 1, 15), new DateTime(2023, 1, 16));

        var cacheKey = "Alpha.BetLists.BalanceUpDayAndMoveBetsDay";
        cacheService.GetOrSet(cacheKey, Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>())
            .Returns(callInfo => callInfo.Arg<Func<BalanceUpDayAndMoveBetsDayModel>>()());

        dynamicRepository.Get<BalanceUpDayAndMoveBetsDayModel>(Arg.Any<GetBalanceUpDayAndMoveBetsDayCriteria>())
            .Returns(reportDateModel);

        // Act
        var result = service.GetBalanceUpDayAndMoveBetsDay();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reportDateModel.BalanceUpDay, result.BalanceUpDay);
        Assert.Equal(reportDateModel.MoveBetsDay, result.MoveBetsDay);
        
        cacheService.Received(1).GetOrSet(
            cacheKey,
            Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(),
            Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public void GetBalanceUpDayAndMoveBetsDay_CallsRepositoryOnce()
    {
        // Arrange
        var reportDateModel = new BalanceUpDayAndMoveBetsDayModel(new DateTime(2023, 1, 15), new DateTime(2023, 1, 16));

        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>())
            .Returns(callInfo => callInfo.Arg<Func<BalanceUpDayAndMoveBetsDayModel>>()());

        dynamicRepository.Get<BalanceUpDayAndMoveBetsDayModel>(Arg.Any<GetBalanceUpDayAndMoveBetsDayCriteria>())
            .Returns(reportDateModel);

        // Act
        service.GetBalanceUpDayAndMoveBetsDay();

        // Assert
        dynamicRepository.Received(1).Get<BalanceUpDayAndMoveBetsDayModel>(Arg.Any<GetBalanceUpDayAndMoveBetsDayCriteria>());
    }

    [Fact]
    public void GetBalanceUpDay_CallsCacheService()
    {
        // Arrange
        var reportDateModel = new BalanceUpDayAndMoveBetsDayModel(new DateTime(2023, 1, 15), new DateTime(2023, 1, 16));

        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>())
            .Returns(reportDateModel);

        // Act
        service.GetBalanceUpDay();

        // Assert
        cacheService.Received(1).GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public void GetMoveBetsDay_CallsCacheService()
    {
        // Arrange
        var reportDateModel = new BalanceUpDayAndMoveBetsDayModel(new DateTime(2023, 1, 15), new DateTime(2023, 1, 16));

        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>())
            .Returns(reportDateModel);

        // Act
        service.GetMoveBetsDay();

        // Assert
        cacheService.Received(1).GetOrSet(Arg.Any<string>(), Arg.Any<Func<BalanceUpDayAndMoveBetsDayModel>>(), Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(service);
    }
}
