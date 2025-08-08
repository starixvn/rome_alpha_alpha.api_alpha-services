using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Services;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class CasinoDetailsServiceTests
{
    private readonly IDynamicRepository mockDynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly IReportDateService mockReportDateService = Substitute.For<IReportDateService>();
    private readonly CasinoDetailsService service;

    public CasinoDetailsServiceTests()
    {
        service = new CasinoDetailsService(mockDynamicRepository, mockReportDateService);
    }

    [Fact]
    public void GetCasinoDetails_WithValidParam_ReturnsPagedData()
    {
        // Arrange
        var param = new CasinoDetailsParam(12345, 100, 1, 10, 1, 20);

        var mockBalanceUpDay = DateTime.Now.AddDays(-1);
        var mockMoveBetsDay = DateTime.Now.AddDays(-2);
        var mockTickets = new List<TicketDto>
        {
            new TicketDto(),
            new TicketDto()
        };

        mockReportDateService.GetMoveBetsDay().Returns(mockMoveBetsDay);
        mockReportDateService.GetBalanceUpDay().Returns(mockBalanceUpDay);
        mockDynamicRepository.Fetch<TicketDto>(Arg.Any<ICriteria>()).Returns(mockTickets);

        // Act
        var result = service.GetCasinoDetails(param);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedData<TicketDto>>(result);
        Assert.Equal(mockTickets, result.Tickets);
        Assert.Equal(param.PageIndex, result.CurrentPageIndex);
        Assert.Equal(param.PageSize, result.PageSize);
    }

    [Fact]
    public void GetCasinoDetails_CallsReportDateServiceMethods()
    {
        // Arrange
        var param = new CasinoDetailsParam(12345, 100, 1, 10, 1, 20);

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockDynamicRepository.Fetch<TicketDto>(Arg.Any<ICriteria>()).Returns(new List<TicketDto>());

        // Act
        service.GetCasinoDetails(param);

        // Assert
        mockReportDateService.Received(1).GetMoveBetsDay();
        mockReportDateService.Received(1).GetBalanceUpDay();
    }

    [Fact]
    public void GetCasinoDetails_CallsDynamicRepositoryWithCorrectCriteria()
    {
        // Arrange
        var param = new CasinoDetailsParam(12345, 100, 1, 10, 1, 20);

        var mockBalanceUpDay = DateTime.Now.AddDays(-1);
        var mockMoveBetsDay = DateTime.Now.AddDays(-2);

        mockReportDateService.GetMoveBetsDay().Returns(mockMoveBetsDay);
        mockReportDateService.GetBalanceUpDay().Returns(mockBalanceUpDay);
        mockDynamicRepository.Fetch<TicketDto>(Arg.Any<ICriteria>()).Returns(new List<TicketDto>());

        // Act
        service.GetCasinoDetails(param);

        // Assert
        mockDynamicRepository.Received(1).Fetch<TicketDto>(Arg.Any<ICriteria>());
    }

    [Fact]
    public void GetCasinoDetails_WithEmptyResult_ReturnsEmptyPagedData()
    {
        // Arrange
        var param = new CasinoDetailsParam(12345, 100, 1, 10, 1, 20);

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockDynamicRepository.Fetch<TicketDto>(Arg.Any<ICriteria>()).Returns(new List<TicketDto>());

        // Act
        var result = service.GetCasinoDetails(param);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Tickets);
    }

    [Fact]
    public void GetCasinoDetails_WithZeroPageSize_HandlesCorrectly()
    {
        // Arrange
        var param = new CasinoDetailsParam(12345, 100, 1, 10, 1, 0);

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockDynamicRepository.Fetch<TicketDto>(Arg.Any<ICriteria>()).Returns(new List<TicketDto>());

        // Act
        var result = service.GetCasinoDetails(param);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.PageSize);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Arrange & Act
        var service = new CasinoDetailsService(mockDynamicRepository, mockReportDateService);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullDynamicRepository_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new CasinoDetailsService(null, mockReportDateService);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullReportDateService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new CasinoDetailsService(mockDynamicRepository, null);
        Assert.NotNull(service);
    }
}
