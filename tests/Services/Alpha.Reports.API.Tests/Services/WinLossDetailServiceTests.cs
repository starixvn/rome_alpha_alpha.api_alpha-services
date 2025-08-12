using Alpha.ApiKey.API.Models;
using Alpha.PartialUM.Services;
using Alpha.Reports.API.DataAccesses;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Services;
using Alpha.Shared.Services.Services;
using GrpcReports;
using NSubstitute;

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

    [Fact]
    public async Task GetMasterWinLossDetail_ValidParam_ReturnsData()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 3,
            ViewerLevelId = 3,
            Level = 2,
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
        winLossDetailDataAccess.GetMasterWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel>
                    {
                        new Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel()
                    },
                    new Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel()
                )
            );

        // Act
        var result = await service.GetMasterWinLossDetail(param);

        // Assert
        Assert.NotEmpty(result.DataSource);
        Assert.NotNull(result.TotalData);
        await winLossDetailDataAccess.Received(1).GetMasterWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>());
    }

    [Fact]
    public async Task GetAgentWinLossDetail_ValidParam_ReturnsData()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 4,
            ViewerLevelId = 4,
            Level = 2, // Changed from 3 to 2 to be within valid range (0-2)
            Site = "site",
            ProductGroup = "2",
            ProductIds = "2,3",
            FromDate = DateTime.Now.AddDays(-7).ToString(),
            ToDate = DateTime.Now.ToString()
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "2,3" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 2, 3 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 2, 3 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel> { new ProductDetailModel() });
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetAgentWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetMemberWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel>
                    {
                        new Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel()
                    },
                    new Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel()
                )
            );

        // Act
        var result = await service.GetAgentWinLossDetail(param);

        // Assert
        Assert.NotEmpty(result.DataSource);
        Assert.NotNull(result.TotalData);
        await winLossDetailDataAccess.Received(1).GetAgentWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetMemberWinLossDetailCriteria>());
    }

    [Fact]
    public async Task GetSuperWinLossDetail_CallsPartialUMService()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 2,
            ViewerLevelId = 2,
            Level = 1,
            Site = "testsite"
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 1 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 1 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel>());
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetSuperWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(),
                    new Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel()
                )
            );

        // Act
        await service.GetSuperWinLossDetail(param);

        // Assert
        partialUMService.Received(1).ThrowIfUM(Arg.Any<string>(), "testsite");
    }

    [Fact]
    public async Task GetMasterWinLossDetail_CallsPartialUMService()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 3,
            ViewerLevelId = 3,
            Level = 2,
            Site = "testsite"
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 1 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 1 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel>());
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetMasterWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel>(),
                    new Alpha.Reports.API.Models.WinLossDetail.MasterWinLossDetailModel()
                )
            );

        // Act
        await service.GetMasterWinLossDetail(param);

        // Assert
        partialUMService.Received(1).ThrowIfUM(Arg.Any<string>(), "testsite");
    }

    [Fact]
    public async Task GetAgentWinLossDetail_CallsPartialUMService()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 4,
            ViewerLevelId = 4,
            Level = 2, // Changed from 3 to 2 to be within valid range (0-2)
            Site = "testsite",
            ProductGroup = "All",
            ProductIds = "1",
            FromDate = DateTime.Now.AddDays(-7).ToString(),
            ToDate = DateTime.Now.ToString()
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 1 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 1 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel>());
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetAgentWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetMemberWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel>(),
                    new Alpha.Reports.API.Models.WinLossDetail.AgentWinLossDetailModel()
                )
            );

        // Act
        await service.GetAgentWinLossDetail(param);

        // Assert
        partialUMService.Received(1).ThrowIfUM(Arg.Any<string>(), "testsite");
    }

    [Fact]
    public async Task GetSuperWinLossDetail_CallsReportDateService()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 2,
            ViewerLevelId = 2,
            Level = 1
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 1 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 1 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel>());
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetSuperWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(),
                    new Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel()
                )
            );

        // Act
        await service.GetSuperWinLossDetail(param);

        // Assert
        reportDateService.Received(1).GetBalanceUpDay();
        reportDateService.Received(1).GetMoveBetsDay();
    }

    [Fact]
    public async Task GetSuperWinLossDetail_CallsProductServices()
    {
        // Arrange
        var request = new GetWinLossDetailReportRequest
        {
            CustId = 1,
            CustLevelId = 2,
            ViewerLevelId = 2,
            Level = 1,
            ProductGroup = "sportsbook",
            ProductIds = "1,2,3"
        };
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1,2" };
        var param = new GetWinLossDetailReportParam(request, settings);

        reportDateService.GetBalanceUpDay().Returns(DateTime.Now);
        reportDateService.GetMoveBetsDay().Returns(DateTime.Now);
        productService.FilterProductIdsByCustomer(Arg.Any<IEnumerable<int>>(), Arg.Any<DateTime>()).Returns(new List<int> { 1, 2 });
        productService.FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), Arg.Any<string>()).Returns(new List<int> { 1, 2 });
        productDetailService.GetProductDetails(Arg.Any<ReportsAppSettingsModel>()).Returns(new List<ProductDetailModel>());
        productDetailService.BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>()).Returns("<xml>");
        winLossDetailDataAccess.GetSuperWinLossDetail(Arg.Any<Alpha.Reports.API.Criteria.GetWinLossDetailCriteria>())
            .Returns(
                new Alpha.Reports.API.Models.ReportDataModel<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(
                    new List<Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel>(),
                    new Alpha.Reports.API.Models.WinLossDetail.SuperWinLossDetailModel()
                )
            );

        // Act
        await service.GetSuperWinLossDetail(param);

        // Assert
        productService.Received(1).FilterProductIdsByCustomer(Arg.Is<IEnumerable<int>>(x => x.Contains(1) && x.Contains(2) && x.Contains(3)), Arg.Any<DateTime>());
        productService.Received(1).FilterProductIdsByGroup(Arg.Any<IEnumerable<int>>(), "sportsbook");
        productDetailService.Received(1).GetProductDetails(settings);
        productDetailService.Received(1).BuildProductDetailsXML(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>());
    }

    [Fact]
    public void Constructor_WithNullServices_DoesNotThrowImmediately()
    {
        // Arrange & Act
        var exception = Record.Exception(() => 
            new WinLossDetailService(null, null, null, null, null));

        // Assert
        Assert.Null(exception);
    }
}
