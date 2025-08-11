using System.Collections.Generic;
using System.Threading.Tasks;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Grpc.Core;
using GrpcReports;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Options;
using Alpha.Reports.API.Services;
using Alpha.ApiKey.API.Models;

namespace Alpha.Reports.API.Tests.Grpc
{
    public class OutstandingGrpcServiceTests
    {
        [Fact]
        public async Task GetSuperOutstanding_ReturnsExpectedResponse()
        {
            // Arrange
            var winLossDetailService = Substitute.For<IWinLossDetailService>();
            var productService = Substitute.For<IProductService>();
            var outstandingService = Substitute.For<IOutstandingService>();
            var settings = Substitute.For<IOptions<ReportsAppSettingsModel>>();
            settings.Value.Returns(new ReportsAppSettingsModel());
            var grpcService = new ReportGrpcService(settings, productService, outstandingService, winLossDetailService);
            var request = new GetOutstandingReportRequest();
            var context = Substitute.For<ServerCallContext>();

            var outstandingList = new List<OutstandingModel>
            {
                new OutstandingModel { CustName = "Test", MemberOutstanding = 100, SuperOutstanding = 200, ToWin = 50, SuperRisk = 30, IsDirectMember = true, MasterOutstanding = 300, AgentOutstanding = 400, MasterRisk = 20 }
            };
            outstandingService.GetReport(Arg.Any<GetOutstandingReportParam>()).Returns(Task.FromResult<IEnumerable<OutstandingModel>>(outstandingList));

            // Act
            var response = await grpcService.GetSuperOutstanding(request, context);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            Assert.Single(response.Report);
            Assert.Equal("Test", response.Report[0].CustName);
        }

        [Fact]
        public async Task GetSuperOnMasterOutstanding_ReturnsExpectedResponse()
        {
            var winLossDetailService = Substitute.For<IWinLossDetailService>();
            var productService = Substitute.For<IProductService>();
            var outstandingService = Substitute.For<IOutstandingService>();
            var settings = Substitute.For<IOptions<ReportsAppSettingsModel>>();
            settings.Value.Returns(new ReportsAppSettingsModel());
            var grpcService = new ReportGrpcService(settings, productService, outstandingService, winLossDetailService);
            var request = new GetOutstandingReportRequest();
            var context = Substitute.For<ServerCallContext>();

            var outstandingList = new List<OutstandingModel>
            {
                new OutstandingModel { CustName = "Test", MemberOutstanding = 100, SuperOutstanding = 200, MasterOutstanding = 300, ToWin = 50, SuperRisk = 30, IsDirectMember = true }
            };
            outstandingService.GetReport(Arg.Any<GetOutstandingReportParam>()).Returns(Task.FromResult<IEnumerable<OutstandingModel>>(outstandingList));

            var response = await grpcService.GetSuperOnMasterOutstanding(request, context);

            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            Assert.Single(response.Report);
            Assert.Equal("Test", response.Report[0].CustName);
        }

        [Fact]
        public async Task GetSuperOnAgentOutstanding_ReturnsExpectedResponse()
        {
            var winLossDetailService = Substitute.For<IWinLossDetailService>();
            var productService = Substitute.For<IProductService>();
            var outstandingService = Substitute.For<IOutstandingService>();
            var settings = Substitute.For<IOptions<ReportsAppSettingsModel>>();
            settings.Value.Returns(new ReportsAppSettingsModel());
            var grpcService = new ReportGrpcService(settings, productService, outstandingService, winLossDetailService);
            var request = new GetOutstandingReportRequest();
            var context = Substitute.For<ServerCallContext>();

            var outstandingList = new List<OutstandingModel>
            {
                new OutstandingModel { CustName = "Test", MemberOutstanding = 100, SuperOutstanding = 200, MasterOutstanding = 300, AgentOutstanding = 400, ToWin = 50, SuperRisk = 30, IsDirectMember = true }
            };
            outstandingService.GetReport(Arg.Any<GetOutstandingReportParam>()).Returns(Task.FromResult<IEnumerable<OutstandingModel>>(outstandingList));

            var response = await grpcService.GetSuperOnAgentOutstanding(request, context);

            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            Assert.Single(response.Report);
            Assert.Equal("Test", response.Report[0].CustName);
        }

        [Fact]
        public async Task GetMasterOutstanding_ReturnsExpectedResponse()
        {
            var winLossDetailService = Substitute.For<IWinLossDetailService>();
            var productService = Substitute.For<IProductService>();
            var outstandingService = Substitute.For<IOutstandingService>();
            var settings = Substitute.For<IOptions<ReportsAppSettingsModel>>();
            settings.Value.Returns(new ReportsAppSettingsModel());
            var grpcService = new ReportGrpcService(settings, productService, outstandingService, winLossDetailService);
            var request = new GetOutstandingReportRequest();
            var context = Substitute.For<ServerCallContext>();

            var outstandingList = new List<OutstandingModel>
            {
                new OutstandingModel { CustName = "Test", MemberOutstanding = 100, MasterOutstanding = 300, ToWin = 50, MasterRisk = 20, IsDirectMember = true }
            };
            outstandingService.GetReport(Arg.Any<GetOutstandingReportParam>()).Returns(Task.FromResult<IEnumerable<OutstandingModel>>(outstandingList));

            var response = await grpcService.GetMasterOutstanding(request, context);

            Assert.NotNull(response);
            Assert.NotNull(response.Total);
            Assert.Single(response.Report);
            Assert.Equal("Test", response.Report[0].CustName);
        }
    }
}
