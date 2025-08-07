using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Services;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Criteria;
using Alpha.PartialUM.Services;
using Fanex.Data.Repository;
using Fanex.BetList.Core.Entities;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class RunningBetListServiceTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly IPartialUMService partialUMService;
    private readonly RunningBetListService service;

    public RunningBetListServiceTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        partialUMService = Substitute.For<IPartialUMService>();
        service = new RunningBetListService(dynamicRepository, partialUMService);
    }

    [Fact]
    public async Task GetBetList_WithValidParam_ReturnsBetListData()
    {
        // Arrange
        var param = new GetRunningBetListParam(
            123,
            "TestCustomer",
            "TestSite",
            new List<int> { 1, 2 });

        var ticketDtos = new List<TicketDto>
        {
            new TicketDto { TransId = 1 },
            new TicketDto { TransId = 2 }
        };

        var ticketData = new List<TicketData>
        {
            new TicketData { RefNo = "1" },
            new TicketData { RefNo = "2" }
        };

        var repositoryResult = (ticketDtos.AsEnumerable(), ticketData.AsEnumerable());

        dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListRunningByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Tickets.Count());
        Assert.Equal(2, result.TicketData.Count);
        Assert.Equal(ticketDtos, result.Tickets);
    }

    [Fact]
    public async Task GetBetList_CallsPartialUMServiceWithCorrectParameters()
    {
        // Arrange
        var param = new GetRunningBetListParam(
            123,
            "TestCustomer",
            "TestSite",
            new List<int> { 1, 2 });

        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListRunningByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        partialUMService.Received(1).ThrowIfUM(Arg.Any<string>(), param.SiteName);
    }

    [Fact]
    public async Task GetBetList_CallsRepositoryWithCorrectCriteria()
    {
        // Arrange
        var param = new GetRunningBetListParam(
            123,
            "TestCustomer",
            "TestSite",
            new List<int> { 1, 2 });

        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListRunningByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        await dynamicRepository.Received(1).FetchMultipleAsync<TicketDto, TicketData>(
            Arg.Is<GetBetListRunningByDateCriteria>(criteria => 
                criteria.CustId == param.CustId &&
                criteria.ProductIds == "1,2"));
    }

    [Fact]
    public async Task GetBetList_WithEmptyTicketData_ReturnsEmptyBetListData()
    {
        // Arrange
        var param = new GetRunningBetListParam(
            123,
            "TestCustomer",
            "TestSite",
            new List<int> { 1 });

        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListRunningByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Tickets);
        Assert.Empty(result.TicketData);
    }

    [Fact]
    public async Task GetBetList_WithSingleProductId_ProcessesCorrectly()
    {
        // Arrange
        var param = new GetRunningBetListParam(
            456,
            "TestCustomer",
            "SingleProductSite",
            new List<int> { 5 });

        var ticketDtos = new List<TicketDto> { new TicketDto { TransId = 100 } };
        var ticketData = new List<TicketData> { new TicketData { RefNo = "100" } };

        var repositoryResult = (ticketDtos.AsEnumerable(), ticketData.AsEnumerable());

        dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListRunningByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        var result = await service.GetBetList(param);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Tickets);
        Assert.Single(result.TicketData);
        await dynamicRepository.Received(1).FetchMultipleAsync<TicketDto, TicketData>(
            Arg.Is<GetBetListRunningByDateCriteria>(criteria => criteria.ProductIds == "5"));
    }

    [Fact]
    public async Task GetBetList_WithMultipleProductIds_JoinsProductIdsCorrectly()
    {
        // Arrange
        var param = new GetRunningBetListParam(
            789,
            "TestCustomer",
            "MultiProductSite",
            new List<int> { 1, 3, 5, 7 });

        var repositoryResult = (Enumerable.Empty<TicketDto>(), Enumerable.Empty<TicketData>());

        dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(Arg.Any<GetBetListRunningByDateCriteria>())
            .Returns(repositoryResult);

        // Act
        await service.GetBetList(param);

        // Assert
        await dynamicRepository.Received(1).FetchMultipleAsync<TicketDto, TicketData>(
            Arg.Is<GetBetListRunningByDateCriteria>(criteria => criteria.ProductIds == "1,3,5,7"));
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullDynamicRepository_DoesNotThrowException()
    {
        // Act & Assert - Constructor doesn't validate parameters
        var testService = new RunningBetListService(null, partialUMService);
        Assert.NotNull(testService);
    }

    [Fact]
    public void Constructor_WithNullPartialUMService_DoesNotThrowException()
    {
        // Act & Assert - Constructor doesn't validate parameters
        var testService = new RunningBetListService(dynamicRepository, null);
        Assert.NotNull(testService);
    }
}
