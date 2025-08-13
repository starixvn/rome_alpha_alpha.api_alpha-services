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

namespace Alpha.BetLists.API.Tests.Grpc
{
    public class WinLossBetListGrpcServiceTests
    {
        private readonly BetListsGrpcService service;
        private readonly IProductService productService;
        private readonly IWinLossBetListService winLossBetListService;
        private readonly IIPInfoService ipInfoService;
        private readonly IBetListHTMLBuilder builder;

        public WinLossBetListGrpcServiceTests()
        {
            productService = Substitute.For<IProductService>();
            var runningBetListService = Substitute.For<IRunningBetListService>();
            winLossBetListService = Substitute.For<IWinLossBetListService>();
            ipInfoService = Substitute.For<IIPInfoService>();
            var casinoDetailsService = Substitute.For<ICasinoDetailsService>();
            var parlayService = Substitute.For<IParlayService>();
            builder = Substitute.For<IBetListHTMLBuilder>();
            var ticketHelper = Substitute.For<ITicketHelper>();
            var betBuilderService = Substitute.For<IBetBuilderService>();
            var logger = Substitute.For<Fanex.Logging.ILogger>();

            var settings = Options.Create(new BetListsAppSettingsModel());
            var ticketDetailsMapper = Options.Create(new TicketDetailsMapperModel 
            { 
                Values = new Dictionary<string, int>(),
                ParlayProducts = new Dictionary<string, byte>()
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
        public async Task SuperViewWinLossBetList_WithValidRequest_CallsExpectedServices()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 123,
                RoleId = 4,
                FromDate = "2023-01-01",
                ToDate = "2023-01-31",
                CustName = "TestUser",
                Site = "TestSite",
                ProductIds = "1,2,3",
                ViewerId = 456
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

            productService.GetAllProductIds(Arg.Any<string>()).Returns(new List<int> { 1, 2, 3 });
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewWinLossBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            
            await winLossBetListService.Received(1).GetBetList(Arg.Any<GetWinLossBetListParam>());
            await ipInfoService.Received(1).GetCountries(Arg.Any<IEnumerable<string>>());
        }

        [Fact]
        public async Task SuperViewWinLossBetList_WithEmptyProductIds_UsesAllProductIds()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 123,
                RoleId = 4,
                ProductIds = "",
                ProductGroup = "Sports"
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

            productService.GetAllProductIds("Sports").Returns(new List<int> { 1, 2, 3, 4 });
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            await service.SuperViewWinLossBetList(request, context);

            // Assert
            productService.Received(1).GetAllProductIds("Sports");
        }

        [Fact]
        public async Task MasterViewWinLossBetList_WithEmptyProductIds_UsesAllProductIds()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 123,
                RoleId = 3,
                ProductIds = "",
                ProductGroup = "LiveCasino"
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

            productService.GetAllProductIds("LiveCasino").Returns(new List<int> { 8, 9 });
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            await service.MasterViewWinLossBetList(request, context);

            // Assert
            productService.Received(1).GetAllProductIds("LiveCasino");
        }

        [Fact]
        public async Task AgentViewWinLossBetList_WithEmptyProductIds_UsesAllProductIds()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 123,
                RoleId = 2,
                ProductIds = "",
                ProductGroup = "Virtual"
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());

            productService.GetAllProductIds("Virtual").Returns(new List<int> { 10, 11 });
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            await service.AgentViewWinLossBetList(request, context);

            // Assert
            productService.Received(1).GetAllProductIds("Virtual");
        }

        [Fact]
        public async Task SuperViewWinLossBetList_CreatesCorrectParamObject()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 456,
                RoleId = 4,
                FromDate = "2023-02-01",
                ToDate = "2023-02-28",
                CustName = "TestCustomer",
                Site = "TestSite",
                ProductIds = "5,6,7",
                ViewerId = 789
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            await service.SuperViewWinLossBetList(request, context);

            // Assert
            await winLossBetListService.Received(1).GetBetList(
                Arg.Is<GetWinLossBetListParam>(p => 
                    p.CustId == 456 && 
                    p.RoleId == 4 && 
                    p.UserName == "TestCustomer" &&
                    p.SiteName == "TestSite" &&
                    p.ViewerId == 789));
        }

        [Fact]
        public async Task MasterViewWinLossBetList_CallsExpectedServices()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 123,
                RoleId = 3
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewWinLossBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            await winLossBetListService.Received(1).GetBetList(Arg.Any<GetWinLossBetListParam>());
            await ipInfoService.Received(1).GetCountries(Arg.Any<IEnumerable<string>>());
        }

        [Fact]
        public async Task AgentViewWinLossBetList_CallsExpectedServices()
        {
            // Arrange
            var request = new GetWinLossBetListRequest
            {
                CustId = 123,
                RoleId = 2
            };

            var betListData = new BetListData(new List<TicketDto>(), new List<ITicketData>());
            winLossBetListService.GetBetList(Arg.Any<GetWinLossBetListParam>()).Returns(betListData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>())
                .Returns(new Dictionary<string, string>());

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.AgentViewWinLossBetList(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            await winLossBetListService.Received(1).GetBetList(Arg.Any<GetWinLossBetListParam>());
            await ipInfoService.Received(1).GetCountries(Arg.Any<IEnumerable<string>>());
        }
    }
}
