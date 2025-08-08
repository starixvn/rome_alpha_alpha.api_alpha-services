using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.Parlay;
using Alpha.BetLists.API.Services;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class ParlayServiceTests
{
    private readonly IReportDateService mockReportDateService = Substitute.For<IReportDateService>();
    private readonly IDynamicRepository mockDynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly ParlayService service;

    public ParlayServiceTests()
    {
        service = new ParlayService(mockReportDateService, mockDynamicRepository);
    }

    [Fact]
    public async Task GetParlayBetList_WithValidParameters_ReturnsTickets()
    {
        // Arrange
        long transId = 12345;
        long custId = 100;
        var expectedTickets = new List<TicketDto>
        {
            new TicketDto(),
            new TicketDto()
        };

        mockDynamicRepository.FetchAsync<TicketDto>(Arg.Any<ICriteria>()).Returns(expectedTickets);

        // Act
        var result = await service.GetParlayBetList(transId, custId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTickets, result);
    }

    [Fact]
    public async Task GetParlayBetList_CallsDynamicRepositoryWithCorrectCriteria()
    {
        // Arrange
        long transId = 12345;
        long custId = 100;

        mockDynamicRepository.FetchAsync<TicketDto>(Arg.Any<ICriteria>()).Returns(new List<TicketDto>());

        // Act
        await service.GetParlayBetList(transId, custId);

        // Assert
        await mockDynamicRepository.Received(1).FetchAsync<TicketDto>(Arg.Any<ICriteria>());
    }

    [Fact]
    public async Task GetParlayBetList_WithEmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        long transId = 12345;
        long custId = 100;

        mockDynamicRepository.FetchAsync<TicketDto>(Arg.Any<ICriteria>()).Returns(new List<TicketDto>());

        // Act
        var result = await service.GetParlayBetList(transId, custId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetComboParlayDetail_WithValidParameters_ReturnsComboParlayDetailData()
    {
        // Arrange
        long refNo = 12345;
        int custId = 100;
        var balanceUpDay = DateTime.Now.AddDays(-1);
        var moveBetsDay = DateTime.Now.AddDays(-2);

        var comboParlayDetails = new List<ComboParlayDetail>
        {
            new ComboParlayDetail()
        };
        var matchDetails = new List<MatchDetail>
        {
            new MatchDetail()
        };

        mockReportDateService.GetBalanceUpDay().Returns(balanceUpDay);
        mockReportDateService.GetMoveBetsDay().Returns(moveBetsDay);
        mockDynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>())
            .Returns((comboParlayDetails, matchDetails));

        // Act
        var result = await service.GetComboParlayDetail(refNo, custId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ComboParlayDetailData>(result);
    }

    [Fact]
    public async Task GetComboParlayDetail_CallsReportDateServiceMethods()
    {
        // Arrange
        long refNo = 12345;
        int custId = 100;

        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockDynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>())
            .Returns((new List<ComboParlayDetail>(), new List<MatchDetail>()));

        // Act
        await service.GetComboParlayDetail(refNo, custId);

        // Assert
        mockReportDateService.Received(1).GetBalanceUpDay();
        mockReportDateService.Received(1).GetMoveBetsDay();
    }

    [Fact]
    public async Task GetComboParlayDetail_CallsDynamicRepositoryWithCorrectCriteria()
    {
        // Arrange
        long refNo = 12345;
        int custId = 100;

        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockDynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>())
            .Returns((new List<ComboParlayDetail>(), new List<MatchDetail>()));

        // Act
        await service.GetComboParlayDetail(refNo, custId);

        // Assert
        await mockDynamicRepository.Received(1).FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>());
    }

    [Fact]
    public async Task GetComboParlayDetail_WithZeroRefNo_StillCallsService()
    {
        // Arrange
        long refNo = 0;
        int custId = 100;

        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockDynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>())
            .Returns((new List<ComboParlayDetail>(), new List<MatchDetail>()));

        // Act
        var result = await service.GetComboParlayDetail(refNo, custId);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>());
    }

    [Fact]
    public async Task GetComboParlayDetail_WithNegativeCustId_StillCallsService()
    {
        // Arrange
        long refNo = 12345;
        int custId = -1;

        mockReportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        mockDynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>())
            .Returns((new List<ComboParlayDetail>(), new List<MatchDetail>()));

        // Act
        var result = await service.GetComboParlayDetail(refNo, custId);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).FetchMultipleAsync<ComboParlayDetail, MatchDetail>(Arg.Any<ICriteria>());
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Arrange & Act
        var service = new ParlayService(mockReportDateService, mockDynamicRepository);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullReportDateService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new ParlayService(null, mockDynamicRepository);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullDynamicRepository_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new ParlayService(mockReportDateService, null);
        Assert.NotNull(service);
    }
}
