using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpha.PartialUM.Services;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.DataAccesses;
using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Services;
using NSubstitute;
using Xunit;
using System;
using Alpha.ApiKey.API.Models;
using GrpcReports;
using Alpha.Reports.API.Models;

namespace Alpha.Reports.API.Tests.Services;

public class WinLossDetailServiceTests
{
    private readonly IPartialUMService partialUMService = Substitute.For<IPartialUMService>();
    private readonly IProductService productService = Substitute.For<IProductService>();
    private readonly IProductDetailService productDetailService = Substitute.For<IProductDetailService>();
    private readonly IReportDateService reportDateService = Substitute.For<IReportDateService>();
    private readonly IWinLossDetailDataAccess winLossDetailDataAccess = Substitute.For<IWinLossDetailDataAccess>();
    private readonly WinLossDetailService service;

    public WinLossDetailServiceTests()
    {
        service = new WinLossDetailService(partialUMService, productService, productDetailService, reportDateService, winLossDetailDataAccess);
    }

    [Fact]
    public async Task GetSuperWinLossDetail_InvalidParam_ReturnsDefault()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 0, // invalid
            CustLevelId = 0,
            ViewerLevelId = 0,
            Level = 0
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1,2" };
        var param = new GetWinLossDetailReportParam(request, settings);

        // Act
        var result = await service.GetSuperWinLossDetail(param);

        // Assert
        Assert.Empty(result.DataSource);
        Assert.NotNull(result.TotalData);
    }

    [Fact]
    public async Task GetSuperWinLossDetail_ValidParam_ReturnsData()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 2,
            ViewerLevelId = 2,
            Level = 1,
            Site = "site",
            ProductGroup = "1",
            ProductIds = "1,2",
            FromDate = DateTime.Now.ToString(),
            ToDate = DateTime.Now.ToString()
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1,2" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 1 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 1 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel> { new ProductDetailModel() });
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetSuperWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>
                    {
                        new Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel()
                    },
                    new Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel()
                )
            );

        // Act
        var result = await service.GetSuperWinLossDetail(param);

        // Assert
        Assert.NotEmpty(result.DataSource);
        Assert.NotNull(result.TotalData);
    }

    [Fact]
    public async Task GetMasterWinLossDetail_InvalidParam_ReturnsDefault()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 0, // invalid
            CustLevelId = 0,
            ViewerLevelId = 0,
            Level = 0
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1,2" };
        var param = new GetWinLossDetailReportParam(request, settings);

        // Act
        var result = await service.GetMasterWinLossDetail(param);

        // Assert
        Assert.Empty(result.DataSource);
        Assert.NotNull(result.TotalData);
    }

    [Fact]
    public async Task GetAgentWinLossDetail_InvalidParam_ReturnsDefault()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 0, // invalid
            CustLevelId = 0,
            ViewerLevelId = 0,
            Level = 0
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1,2" };
        var param = new GetWinLossDetailReportParam(request, settings);

        // Act
        var result = await service.GetAgentWinLossDetail(param);

        // Assert
        Assert.Empty(result.DataSource);
        Assert.NotNull(result.TotalData);
    }
}
