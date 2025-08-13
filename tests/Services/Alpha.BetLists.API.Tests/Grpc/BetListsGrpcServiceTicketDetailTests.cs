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
    public class BetListsGrpcServiceTicketDetailTests
    {
        private readonly BetListsGrpcService service;
        private readonly ICasinoDetailsService casinoDetailsService;

        public BetListsGrpcServiceTicketDetailTests()
        {
            casinoDetailsService = Substitute.For<ICasinoDetailsService>();
            var builder = Substitute.For<IBetListHTMLBuilder>();
            var ipInfoService = Substitute.For<IIPInfoService>();

            var appSettings = Substitute.For<IOptions<BetListsAppSettingsModel>>();
            appSettings.Value.Returns(new BetListsAppSettingsModel());

            var ticketDetailsMapper = Substitute.For<IOptions<TicketDetailsMapperModel>>();
            ticketDetailsMapper.Value.Returns(new TicketDetailsMapperModel
            {
                Values = new Dictionary<string, int>
                {
                    { "test", 1 },
                    { "casino", 8 }  // Add ProductId 8 for casino tests
                }
            });

            var productService = Substitute.For<IProductService>();
            var ticketHelper = Substitute.For<ITicketHelper>();
            var winLossBetListService = Substitute.For<IWinLossBetListService>();
            var runningBetListService = Substitute.For<IRunningBetListService>();
            var betBuilderService = Substitute.For<IBetBuilderService>();
            var parlayService = Substitute.For<IParlayService>();
            var logger = Substitute.For<Fanex.Logging.ILogger>();

            // Setup default mock returns to prevent null reference exceptions
            var emptyPagedData = new PagedData<TicketDto>(new List<TicketDto>(), 1, 10, 0);
            casinoDetailsService.GetCasinoDetails(Arg.Any<CasinoDetailsParam>()).Returns(emptyPagedData);
            ipInfoService.GetCountries(Arg.Any<IEnumerable<string>>()).Returns(new Dictionary<string, string>());

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
        public async Task SuperViewTicketDetails_WithValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 1,
                TransType = 1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public async Task MasterViewTicketDetails_WithValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 1,
                TransType = 1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public async Task AgentViewTicketDetails_WithValidRequest_ReturnsResponse()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 1,
                TransType = 1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.AgentViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public async Task SuperViewTicketDetails_WithCasinoProduct_CallsCasinoDetailsService()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 8, // Casino product
                TransType = 1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
            casinoDetailsService.Received(1).GetCasinoDetails(Arg.Any<Alpha.BetLists.API.Params.CasinoDetailsParam>());
        }

        [Fact]
        public async Task MasterViewTicketDetails_WithCasinoProduct_CallsCasinoDetailsService()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 8, // Casino product
                TransType = 1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
            casinoDetailsService.Received(1).GetCasinoDetails(Arg.Any<Alpha.BetLists.API.Params.CasinoDetailsParam>());
        }

        [Fact]
        public async Task AgentViewTicketDetails_WithCasinoProduct_CallsCasinoDetailsService()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 8, // Casino product
                TransType = 1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.AgentViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
            casinoDetailsService.Received(1).GetCasinoDetails(Arg.Any<Alpha.BetLists.API.Params.CasinoDetailsParam>());
        }

        [Fact]
        public async Task SuperViewTicketDetails_WithZeroRefNo_ReturnsEmptyResponse()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 1,
                TransType = 1,
                RefNo = 0,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.SuperViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.TicketDetails);
        }

        [Fact]
        public async Task MasterViewTicketDetails_WithInvalidTransType_ReturnsEmptyResponse()
        {
            // Arrange
            var request = new GetTicketDetailsRequest
            {
                ProductId = 1,
                TransType = -1,
                RefNo = 12345,
                CustId = 123
            };

            var context = Substitute.For<ServerCallContext>();

            // Act
            var response = await service.MasterViewTicketDetails(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.Empty(response.TicketDetails);
        }
    }
}
