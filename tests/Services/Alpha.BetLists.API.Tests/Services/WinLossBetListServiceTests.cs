using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Services;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Criteria;
using Alpha.BetLists.API.Tests.Helpers;
using Alpha.PartialUM.Services;
using Alpha.Shared.Services.Services;
using Fanex.Data.Repository;
using Fanex.BetList.Core.Entities;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class WinLossBetListServiceTests
{
    private readonly IDynamicRepository mockDynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly IPartialUMService mockPartialUMService = Substitute.For<IPartialUMService>();
    private readonly IReportDateService mockReportDateService = Substitute.For<IReportDateService>();
    private readonly IProductDetailService mockProductDetailService = Substitute.For<IProductDetailService>();
    private readonly WinLossBetListService service;

    public WinLossBetListServiceTests()
    {
        service = new WinLossBetListService(
            mockDynamicRepository,
            mockPartialUMService,
            mockReportDateService,
            mockProductDetailService);
    }

    [Fact]
    public async Task GetBetList_WithValidParam_ReturnsCorrectBetListData()
    {
        // Arrange
        var param = CreateValidParam();
        var tickets = new List<TicketDto>
        {
            new TicketDto { TransId = 1, RefNo = "TestRef1" },
            new TicketDto { TransId = 2, RefNo = "TestRef2" }
        };
        var ticketData = new List<TicketData>
        {
            new TicketData { RefNo = "TestRef1", Status = "Win" },
            new TicketData { RefNo = "TestRef2", Status = "Lose" }
        };
        var repositoryResult = (tickets.AsEnumerable(), ticketData.AsEnumerable());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Tickets.Count());
        Assert.Equal(2, result.TicketData.Count());
        Assert.NotNull(result.BetlistOutputParams);
    }

    [Fact]
    public async Task GetBetList_WithEmptyRepositoryResult_ReturnsEmptyBetListData()
    {
        // Arrange
        var param = CreateValidParam();
        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Tickets);
        Assert.Empty(result.TicketData);
        Assert.NotNull(result.BetlistOutputParams);
    }

    [Fact]
    public async Task GetBetList_CallsPartialUMServiceWithCorrectParameters()
    {
        // Arrange
        var param = CreateValidParam();
        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        mockPartialUMService.Received(1).ThrowIfUM(Arg.Any<string>(), param.SiteName);
    }

    [Fact]
    public async Task GetBetList_CallsReportDateServiceGetMoveBetsDay()
    {
        // Arrange
        var param = CreateValidParam();
        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        mockReportDateService.Received(1).GetMoveBetsDay();
    }

    [Fact]
    public async Task GetBetList_CallsProductDetailServiceWithProductIds()
    {
        // Arrange
        var param = CreateValidParam();
        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        mockProductDetailService.Received(1).BuildProductDetailsXML(param.ProductIds);
    }

    [Fact]
    public async Task GetBetList_CallsDynamicRepositoryWithCorrectCriteria()
    {
        // Arrange
        var param = CreateValidParam();
        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());
        var moveBetsDay = DateTime.Now.AddDays(-1);

        mockReportDateService.GetMoveBetsDay().Returns(moveBetsDay);
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        await mockDynamicRepository.Received(1).FetchMultipleAsync<TicketDto, TicketData>(
            Arg.Is<GetBetListWinLossByDateCriteria>(criteria => 
                criteria.CustId == param.CustId &&
                criteria.FromDate == param.FromDate &&
                criteria.ToDate == param.ToDate &&
                criteria.MaxTotalRow == param.MaxTotalRow));
    }

    [Fact]
    public async Task GetBetList_WithMultipleProductIds_JoinsProductIdsCorrectly()
    {
        // Arrange
        var param = CreateValidParamWithMultipleProducts();
        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        await mockDynamicRepository.Received(1).FetchMultipleAsync<TicketDto, TicketData>(
            Arg.Is<GetBetListWinLossByDateCriteria>(criteria => criteria.ProductIds == "1,2,3"));
    }

    [Fact]
    public async Task GetBetList_WithErrorFromRepository_ReturnsBetListDataWithError()
    {
        // Arrange
        var param = CreateValidParam();
        var tickets = new List<TicketDto>();
        var ticketData = new List<TicketData>();
        var repositoryResult = (tickets.AsEnumerable(), ticketData.AsEnumerable());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.BetlistOutputParams);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Arrange & Act
        var testService = new WinLossBetListService(
            mockDynamicRepository,
            mockPartialUMService,
            mockReportDateService,
            mockProductDetailService);

        // Assert
        Assert.NotNull(testService);
    }

    [Fact]
    public void Constructor_WithNullDynamicRepository_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var testService = new WinLossBetListService(
            null,
            mockPartialUMService,
            mockReportDateService,
            mockProductDetailService);
        Assert.NotNull(testService);
    }

    [Fact]
    public void Constructor_WithNullPartialUMService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var testService = new WinLossBetListService(
            mockDynamicRepository,
            null,
            mockReportDateService,
            mockProductDetailService);
        Assert.NotNull(testService);
    }

    [Fact]
    public void Constructor_WithNullReportDateService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var testService = new WinLossBetListService(
            mockDynamicRepository,
            mockPartialUMService,
            null,
            mockProductDetailService);
        Assert.NotNull(testService);
    }

    [Fact]
    public void Constructor_WithNullProductDetailService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var testService = new WinLossBetListService(
            mockDynamicRepository,
            mockPartialUMService,
            mockReportDateService,
            null);
        Assert.NotNull(testService);
    }

    [Fact]
    public async Task GetBetList_ReturnsTicketDataAsITicketDataList()
    {
        // Arrange
        var param = CreateValidParam();
        var tickets = new List<TicketDto>
        {
            new TicketDto { TransId = 1, RefNo = "TestRef1" }
        };
        var ticketData = new List<TicketData>
        {
            new TicketData { RefNo = "TestRef1", Status = "Win" }
        };
        var repositoryResult = (tickets.AsEnumerable(), ticketData.AsEnumerable());

        mockReportDateService.GetMoveBetsDay().Returns(DateTime.Now.AddDays(-1));
        mockProductDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>()).Returns("<ROOT></ROOT>");
        mockDynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListWinLossByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ITicketData>>(result.TicketData);
        Assert.Single(result.TicketData);
    }

    private GetWinLossBetListParam CreateValidParam()
    {
        var settings = TestParameterHelper.CreateBetListsAppSettings();
        return new GetWinLossBetListParam(
            custId: 123,
            roleId: 1,
            fromDate: DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
            toDate: DateTime.Now.ToString("yyyy-MM-dd"),
            userName: "TestUser",
            siteName: "TestSite",
            productIds: new List<int> { 1 },
            viewerId: 456,
            settings: settings);
    }

    private GetWinLossBetListParam CreateValidParamWithMultipleProducts()
    {
        var settings = TestParameterHelper.CreateBetListsAppSettings();
        return new GetWinLossBetListParam(
            custId: 789,
            roleId: 2,
            fromDate: DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
            toDate: DateTime.Now.ToString("yyyy-MM-dd"),
            userName: "TestUser2",
            siteName: "TestSite2",
            productIds: new List<int> { 1, 2, 3 },
            viewerId: 789,
            settings: settings);
    }
}
