using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.BetLists.API.Services;
using Alpha.BetLists.API.Params;
using Alpha.IPInfo.Services;
using Fanex.BetDetails.BetBuilder.Services;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using GrpcBetLists;
using Grpc.Core;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Alpha.BetLists.API.Tests.Grpc
{
    public class RunningBetListGrpcServiceTests
    {
        private readonly BetListsGrpcService service;
        private readonly IProductService productService;
        private readonly IRunningBetListService runningBetListService;
        private readonly IIPInfoService ipInfoService;
        private readonly IBetListHTMLBuilder builder;
        private readonly ITicketHelper ticketHelper;

        public RunningBetListGrpcServiceTests()
        {
            productService = Substitute.For<IProductService>();
            runningBetListService = Substitute.For<IRunningBetListService>();
            ipInfoService = Substitute.For<IIPInfoService>();
            builder = Substitute.For<IBetListHTMLBuilder>();
            ticketHelper = Substitute.For<ITicketHelper>();

            var winLossBetListService = Substitute.For<IWinLossBetListService>();
            var casinoDetailsService = Substitute.For<ICasinoDetailsService>();
            var parlayService = Substitute.For<IParlayService>();
            var betBuilderService = Substitute.For<IBetBuilderService>();
            var logger = Substitute.For<Fanex.Logging.ILogger>();

            var appSettings = Substitute.For<IOptions<BetListsAppSettingsModel>>();
            appSettings.Value.Returns(new BetListsAppSettingsModel());

            var ticketDetailsMapper = Substitute.For<IOptions<TicketDetailsMapperModel>>();
            ticketDetailsMapper.Value.Returns(new TicketDetailsMapperModel
            {
                Values = new Dictionary<string, int>
                {
                    { "test", 1 }
                },
                ParlayProducts = new Dictionary<string, byte>()
            });

            // Setup default mocks
            var mockTickets = new List<TicketDto>
            {
                new TicketDto
                {
                    TransId = 123,
                    UserName = "testuser",
                    IP = "192.168.1.1",
                    Status = "Running", // Required for PTCommBuilderBase
                    RefNo = "REF123",
                    CustId = 456,
                    CommStatus = "1",
                    BetTypeId = 1,
                    AgentDiscount = 5.0m,
                    MasterDiscount = 3.0m,
                    SuperDiscount = 1.0m,
                    DeviceType = 1 // 1 = Desktop platform
                }
            };

            var mockTicketData = new List<ITicketData>();
            var mockBetListData = new BetListData(mockTickets, mockTicketData);

            runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(mockBetListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>()).Returns(new Dictionary<string, string>
            {
                { "192.168.1.1", "US" }
            });

            builder.BuildChoice(Arg.Any<TicketDto>(), Arg.Any<ITicketHelper>(), Arg.Any<List<ITicketData>>(), Arg.Any<bool>()).Returns("Test Choice");
            builder.BuildTrans(Arg.Any<TicketDto>()).Returns("2023-01-01 10:00:00");
            builder.BuildOdds(Arg.Any<TicketDto>(), Arg.Any<List<ITicketData>>(), Arg.Any<GetCachePropertyById>()).Returns("1.50");
            builder.BuildStake(Arg.Any<TicketDto>(), Arg.Any<ITicketHelper>()).Returns("100.00");
            builder.BuildStatus(Arg.Any<TicketDto>(), Arg.Any<ITicketHelper>(), Arg.Any<List<ITicketData>>(), Arg.Any<bool>()).Returns("Running");

            service = new BetListsGrpcService(
                appSettings,
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
                CustName = "TestUser",
                Site = "TestSite",
                ProductIds = "1,2,3",
                ProductGroup = "Sports"
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BetLists);
            Assert.Single(response.BetLists);

            var betList = response.BetLists.First();
            Assert.Equal("1", betList.No);
            Assert.Equal("Test Choice", betList.Choice);
            Assert.Equal("1.50", betList.Odds);
            Assert.Equal("100.00", betList.Stake);
            Assert.Equal("Running<div class=\"country\">US</div>", betList.Status);

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
                CustName = "TestUser",
                Site = "TestSite",
                ProductIds = "",
                ProductGroup = "Sports"
            };

            productService.GetAllProductIds("Sports").Returns(new List<int> { 1, 2, 3 });
            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            productService.Received(1).GetAllProductIds("Sports");
        }

        [Fact]
        public async Task MasterViewRunningBetList_WithValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new GetRunningBetListRequest
            {
                CustId = 456,
                CustName = "MasterUser",
                Site = "MasterSite",
                ProductIds = "4,5,6",
                ProductGroup = "Casino"
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BetLists);
            Assert.Single(response.BetLists);

            var betList = response.BetLists.First();
            Assert.Equal("1", betList.No);
            Assert.Equal("Test Choice", betList.Choice);
            Assert.Equal("1.50", betList.Odds);
            Assert.Equal("100.00", betList.Stake);
            Assert.Equal("Running<div class=\"country\">US</div>", betList.Status);
            Assert.Equal("Desktop", betList.Platform);

            await runningBetListService.Received(1).GetBetList(Arg.Any<GetRunningBetListParam>());
        }

        [Fact]
        public async Task MasterViewRunningBetList_WithEmptyProductIds_UsesProductService()
        {
            // Arrange
            var request = new GetRunningBetListRequest
            {
                CustId = 456,
                CustName = "MasterUser",
                Site = "MasterSite",
                ProductIds = "",
                ProductGroup = "LiveCasino"
            };

            productService.GetAllProductIds("LiveCasino").Returns(new List<int> { 7, 8, 9 });
            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            productService.Received(1).GetAllProductIds("LiveCasino");
        }

        [Fact]
        public async Task AgentViewRunningBetList_WithValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new GetRunningBetListRequest
            {
                CustId = 789,
                CustName = "AgentUser",
                Site = "AgentSite",
                ProductIds = "10,11,12",
                ProductGroup = "Sports"
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.AgentViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BetLists);
            Assert.Single(response.BetLists);

            var betList = response.BetLists.First();
            Assert.Equal("1", betList.No);
            Assert.Equal("Test Choice", betList.Choice);
            Assert.Equal("1.50", betList.Odds);
            Assert.Equal("100.00", betList.Stake);
            Assert.Equal("Running<div class=\"country\">US</div>", betList.Status);
            Assert.Equal("Desktop", betList.Platform);

            await runningBetListService.Received(1).GetBetList(Arg.Any<GetRunningBetListParam>());
        }

        [Fact]
        public async Task AgentViewRunningBetList_WithEmptyProductIds_UsesProductService()
        {
            // Arrange
            var request = new GetRunningBetListRequest
            {
                CustId = 789,
                CustName = "AgentUser",
                Site = "AgentSite",
                ProductIds = "",
                ProductGroup = "RNG"
            };

            productService.GetAllProductIds("RNG").Returns(new List<int> { 13, 14, 15 });
            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.AgentViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            productService.Received(1).GetAllProductIds("RNG");
        }

        [Fact]
        public async Task SuperViewRunningBetList_WithMultipleTickets_ReturnsCorrectCount()
        {
            // Arrange
            var multipleTickets = new List<TicketDto>
            {
                new TicketDto 
                { 
                    TransId = 1, 
                    UserName = "user1", 
                    IP = "192.168.1.1",
                    Status = "Running",
                    RefNo = "REF1",
                    CustId = 456,
                    CommStatus = "1",
                    BetTypeId = 1,
                    AgentDiscount = 5.0m,
                    MasterDiscount = 3.0m,
                    SuperDiscount = 1.0m
                },
                new TicketDto 
                { 
                    TransId = 2, 
                    UserName = "user2", 
                    IP = "192.168.1.2",
                    Status = "Running",
                    RefNo = "REF2",
                    CustId = 456,
                    CommStatus = "1",
                    BetTypeId = 1,
                    AgentDiscount = 5.0m,
                    MasterDiscount = 3.0m,
                    SuperDiscount = 1.0m
                },
                new TicketDto 
                { 
                    TransId = 3, 
                    UserName = "user3", 
                    IP = "192.168.1.3",
                    Status = "Running",
                    RefNo = "REF3",
                    CustId = 456,
                    CommStatus = "1",
                    BetTypeId = 1,
                    AgentDiscount = 5.0m,
                    MasterDiscount = 3.0m,
                    SuperDiscount = 1.0m
                }
            };

            var mockBetListData = new BetListData(multipleTickets, new List<ITicketData>());
            runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(mockBetListData);

            var request = new GetRunningBetListRequest
            {
                CustId = 123,
                CustName = "TestUser",
                Site = "TestSite",
                ProductIds = "1,2,3"
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(3, response.BetLists.Count);
            Assert.Equal("1", response.BetLists[0].No);
            Assert.Equal("2", response.BetLists[1].No);
            Assert.Equal("3", response.BetLists[2].No);
        }

        [Fact]
        public async Task MasterViewRunningBetList_WithNoTickets_ReturnsEmptyList()
        {
            // Arrange
            var emptyBetListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());
            runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(emptyBetListData);

            var request = new GetRunningBetListRequest
            {
                CustId = 456,
                CustName = "MasterUser",
                Site = "MasterSite",
                ProductIds = "1,2,3"
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.BetLists);
        }

        [Fact]
        public async Task AgentViewRunningBetList_WithNullIP_HandlesGracefully()
        {
            // Arrange
            var ticketsWithNullIP = new List<TicketDto>
            {
                new TicketDto { 
                    TransId = 1, 
                    UserName = "user1", 
                    IP = null,
                    Status = "Running",
                    RefNo = "REF001",
                    CommStatus = "Active",
                    AgentDiscount = 0.02m,
                    MasterDiscount = 0.05m,
                    SuperDiscount = 0.1m
                },
                new TicketDto { 
                    TransId = 2, 
                    UserName = "user2", 
                    IP = "",
                    Status = "Running",
                    RefNo = "REF002",
                    CommStatus = "Active",
                    AgentDiscount = 0.02m,
                    MasterDiscount = 0.05m,
                    SuperDiscount = 0.1m
                },
                new TicketDto { 
                    TransId = 3, 
                    UserName = "user3", 
                    IP = "  ",
                    Status = "Running",
                    RefNo = "REF003",
                    CommStatus = "Active",
                    AgentDiscount = 0.02m,
                    MasterDiscount = 0.05m,
                    SuperDiscount = 0.1m
                }
            };

            var mockBetListData = new BetListData(ticketsWithNullIP, new List<ITicketData>());
            runningBetListService.GetBetList(Arg.Any<GetRunningBetListParam>()).Returns(mockBetListData);

            var request = new GetRunningBetListRequest
            {
                CustId = 789,
                CustName = "AgentUser",
                Site = "AgentSite",
                ProductIds = "1,2,3"
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.AgentViewRunningBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(3, response.BetLists.Count);
            // Should not call GetCountries with empty IP list
            await ipInfoService.Received(1).GetCountries(Arg.Is<IEnumerable<string>>(ips => !ips.Any()));
        }
    }
}
