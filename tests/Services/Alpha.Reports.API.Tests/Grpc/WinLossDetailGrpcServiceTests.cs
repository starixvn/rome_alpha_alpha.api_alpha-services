using System.Collections.Generic;
using System.Threading.Tasks;
using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Services;
using Alpha.ApiKey.API.Models;
using Grpc.Core;
using GrpcReports;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Alpha.Reports.API.Tests.Grpc
{
    public class WinLossDetailGrpcServiceTests
    {
        [Fact]
        public async Task GetSuperWinLossDetail_ReturnsExpectedResponse()
        {
            // Arrange
            var winLossDetailService = Substitute.For<IWinLossDetailService>();
            var productService = Substitute.For<IProductService>();
            var outstandingService = Substitute.For<IOutstandingService>();
            var settings = Substitute.For<IOptions<ReportsAppSettingsModel>>();
            settings.Value.Returns(new ReportsAppSettingsModel());
            var grpcService = new ReportGrpcService(settings, productService, outstandingService, winLossDetailService);
            var request = new GetWinLossDetailReportRequest();
            var context = Substitute.For<ServerCallContext>();

            var totalData = new Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel
            {
                BetCount = 10,
                Turnover = 1000m,
                MemberWinLoss = 500m,
                MemberComm = 50m,
                AgentWinLoss = 200m,
                AgentComm = 20m,
                MasterWinLoss = 100m,
                MasterComm = 10m,
                SuperWinLoss = 50m,
                SuperComm = 5m,
                NetTurnover = 900m,
                GrossComm = 15m,
                RewardAmount = 5m,
                RewardCurrency = "USD"
            };

            var reportData = new ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(new List<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(), totalData);
            winLossDetailService.GetSuperWinLossDetail(Arg.Any<GetWinLossDetailReportParam>()).Returns(Task.FromResult(reportData));

            // Act
            var response = await grpcService.GetSuperWinLossDetail(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("USD", response.Total.RewardCurrency);
            Assert.Equal("1,000.00", response.Total.Turnover);
        }
    }
}
