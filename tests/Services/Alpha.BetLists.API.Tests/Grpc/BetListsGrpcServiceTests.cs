using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Services;
using Alpha.IPInfo.Services;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Grpc.Core;
using GrpcBetLists;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Grpc;

public class BetListsGrpcServiceTests
{
    private readonly BetListsGrpcService service;
    private readonly IProductService productService;
    private readonly IRunningBetListService runningBetListService;
    private readonly IWinLossBetListService winLossBetListService;
    private readonly IIPInfoService ipInfoService;
    private readonly ICasinoDetailsService casinoDetailsService;
    private readonly IParlayService parlayService;
    private readonly IBetListHTMLBuilder builder;
    private readonly ITicketHelper ticketHelper;
    private readonly IBetBuilderService betBuilderService;
    private readonly Fanex.Logging.ILogger logger;

    public BetListsGrpcServiceTests()
    {
        productService = Substitute.For<IProductService>();
        runningBetListService = Substitute.For<IRunningBetListService>();
        winLossBetListService = Substitute.For<IWinLossBetListService>();
        ipInfoService = Substitute.For<IIPInfoService>();
        casinoDetailsService = Substitute.For<ICasinoDetailsService>();
        parlayService = Substitute.For<IParlayService>();
        builder = Substitute.For<IBetListHTMLBuilder>();
        ticketHelper = Substitute.For<ITicketHelper>();
        betBuilderService = Substitute.For<IBetBuilderService>();
        logger = Substitute.For<Fanex.Logging.ILogger>();

        var settings = Options.Create(new BetListsAppSettingsModel());
        var ticketDetailsMapper = Options.Create(new TicketDetailsMapperModel 
        { 
            Values = [],
            ParlayProducts = []
        });

        service = new BetListsGrpcService(
            settings,
            ticketDetailsMapper,
            productService,
            builder,
            ticketHelper,
            runningBetListService,
            winLossBetListService,
            ipInfoService,
            casinoDetailsService,
            betBuilderService,
            parlayService,
            logger);
    }

    [Fact]
    public async Task SuperViewRunningBetList_WithValidRequest_ReturnsResponse()
    {
        // Arrange
        var request = new GetRunningBetListRequest
        {
            CustId = 123,
            CustName = "TestCustomer",
            Site = "TestSite",
            ProductGroup = "All",
            ProductIds = "1,2"
        };

        var productIds = new List<int> { 1, 2 };
        var tickets = new List<TicketDto>
        {
            new TicketDto { TransId = 1, IP = "192.168.1.1", Status = "Running" },
            new TicketDto { TransId = 2, IP = "192.168.1.2", Status = "Running" }
        };

        var ticketData = new List<ITicketData>
        {
            new TicketData { RefNo = "1" },
            new TicketData { RefNo = "2" }
        };

        var betListData = new BetListData(tickets, ticketData);
        var countries = new Dictionary<string, string>
        {
            { "192.168.1.1", "US" },
            { "192.168.1.2", "UK" }
        };

        productService.GetAllProductIds(request.ProductGroup).Returns(productIds);
        runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(betListData);
        ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>()).Returns(countries);
        builder.BuildChoice(Arg.Any<TicketDto>(), Arg.Any<ITicketHelper>(), Arg.Any<List<ITicketData>>(), Arg.Any<bool>())
            .Returns("Test Choice");
        builder.BuildTrans(Arg.Any<TicketDto>()).Returns("2023-01-01 12:00:00");
        builder.BuildOdds(Arg.Any<TicketDto>(), Arg.Any<List<ITicketData>>(), Arg.Any<GetCachePropertyById>())
            .Returns("1.5");
        builder.BuildStake(Arg.Any<TicketDto>(), Arg.Any<ITicketHelper>()).Returns("100.00");
        builder.BuildStatus(Arg.Any<TicketDto>(), Arg.Any<ITicketHelper>(), Arg.Any<List<ITicketData>>(), Arg.Any<bool>())
            .Returns("Running");

        var context = Substitute.For<ServerCallContext>();

        // Act
        var result = await service.SuperViewRunningBetList(request, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.BetLists.Count);
        await runningBetListService.Received(1).GetBetList(Arg.Any<GetRunningBetListParam>());
        await ipInfoService.Received(1).GetCountries(Arg.Any<IEnumerable<string>>());
    }

    [Fact]
    public async Task SuperViewRunningBetList_WithEmptyProductIds_UsesProductService()
    {
        // Arrange
        var request = new GetRunningBetListRequest
        {
            CustId = 123,
            CustName = "TestCustomer",
            Site = "TestSite",
            ProductGroup = "Sportsbook",
            ProductIds = ""
        };

        var productIds = new List<int> { 1 };
        var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

        productService.GetAllProductIds(request.ProductGroup).Returns(productIds);
        runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(betListData);
        ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>()).Returns(new Dictionary<string, string>());

        var context = Substitute.For<ServerCallContext>();

        // Act
        await service.SuperViewRunningBetList(request, context);

        // Assert
        productService.Received(1).GetAllProductIds(request.ProductGroup);
        await runningBetListService.Received(1).GetBetList(
            Arg.Is<GetRunningBetListParam>(p => p.ProductIds.SequenceEqual(productIds)));
    }

    [Fact]
    public async Task SuperViewRunningBetList_WithNullProductIds_UsesProductService()
    {
        // Arrange
        var request = new GetRunningBetListRequest
        {
            CustId = 123,
            CustName = "TestCustomer",
            Site = "TestSite",
            ProductGroup = "Casino"
        };

        var productIds = new List<int> { 2, 3, 4 };
        var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

        productService.GetAllProductIds(request.ProductGroup).Returns(productIds);
        runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(betListData);
        ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>()).Returns(new Dictionary<string, string>());

        var context = Substitute.For<ServerCallContext>();

        // Act
        await service.SuperViewRunningBetList(request, context);

        // Assert
        productService.Received(1).GetAllProductIds(request.ProductGroup);
        await runningBetListService.Received(1).GetBetList(
            Arg.Is<GetRunningBetListParam>(p => p.ProductIds.SequenceEqual(productIds)));
    }

    [Fact]
    public async Task SuperViewRunningBetList_WithEmptyTickets_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new GetRunningBetListRequest
        {
            CustId = 123,
            CustName = "TestCustomer",
            Site = "TestSite",
            ProductGroup = "All",
            ProductIds = "1"
        };

        var productIds = new List<int> { 1 };
        var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

        productService.GetAllProductIds(request.ProductGroup).Returns(productIds);
        runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(betListData);
        ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>()).Returns(new Dictionary<string, string>());

        var context = Substitute.For<ServerCallContext>();

        // Act
        var result = await service.SuperViewRunningBetList(request, context);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.BetLists);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => new BetListsGrpcService(
            null,
            Options.Create(new TicketDetailsMapperModel 
            { 
                Values = new Dictionary<string, int>(),
                ParlayProducts = new Dictionary<string, byte>()
            }),
            productService,
            builder,
            ticketHelper,
            runningBetListService,
            winLossBetListService,
            ipInfoService,
            casinoDetailsService,
            betBuilderService,
            parlayService,
            logger));
    }
}
