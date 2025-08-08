using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;
using Alpha.Customer.API.Services;
using Grpc.Core;
using GrpcCustomer;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Alpha.Customer.API.Tests.Grpc;

public class CustomerGrpcServiceTests
{
    private readonly ICustomerService customerService = Substitute.For<ICustomerService>();
    private readonly ILogger<CustomerGrpcService> logger = Substitute.For<ILogger<CustomerGrpcService>>();
    private readonly CustomerGrpcService grpcService;
    private readonly ServerCallContext context = Substitute.For<ServerCallContext>();

    public CustomerGrpcServiceTests()
    {
        grpcService = new CustomerGrpcService(customerService, logger);
    }

    [Fact]
    public async Task GetCustomerStatus_WithValidRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var request = new GetCustomerStatusRequest
        {
            CustId = 12345,
            SubAccId = 0
        };

        var customerStatus = new CustomerStatus(12345, 0, true, false, false, "[E][H]");
        customerService.GetCustomerStatus(Arg.Any<CustomerStatusParam>())
            .Returns(customerStatus);

        // Act
        var response = await grpcService.GetCustomerStatus(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(customerStatus.CustId, response.CustId);
        Assert.Equal(customerStatus.SubAccId, response.SubAccId);
        Assert.Equal(customerStatus.IsActive, response.IsActive);
        Assert.Equal(customerStatus.IsProtectedMode, response.IsProtectedMode);
        Assert.Equal(customerStatus.IsLocked, response.IsLocked);
        Assert.Equal(customerStatus.IsSubAcc, response.IsSubAcc);
        Assert.Equal(customerStatus.HasReportPermission, response.HasReportPermission);
        Assert.Equal(customerStatus.HasMemberInformationPermission, response.HasMemberInformationPermission);
    }

    [Fact]
    public async Task GetCustomerInfo_WithValidRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var request = new GetCustomerInfoRequest
        {
            CustId = 12345,
            Username = "testuser"
        };

        var customerInfo = new CustomerInfoModel(12345, 3, "TestSite", 1);
        customerService.GetCustomerInfo(request.CustId, request.Username)
            .Returns(customerInfo);

        // Act
        var response = await grpcService.GetCustomerInfo(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(customerInfo.CustId, response.CustId);
        Assert.Equal(customerInfo.RoleId, response.RoleId);
        Assert.Equal(customerInfo.Site, response.Site);
        Assert.Equal(customerInfo.Currency, response.Currency);
    }

    [Fact]
    public async Task GetDownlineInfo_WithValidRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var request = new GetDownlineInfoRequest
        {
            ViewerId = 12345,
            DownlineUsername = "downlineuser"
        };

        var downlineInfo = new DownlineInfo(67890, 3, false);
        customerService.GetDownlineInfo(request.ViewerId, request.DownlineUsername)
            .Returns(downlineInfo);

        // Act
        var response = await grpcService.GetDownlineInfo(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsBelonging);
    }

    [Fact]
    public async Task GetBelongingInfo_WithValidRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var request = new GetBeLongingInfoRequest
        {
            DownlineId = 12345,
            ViewerId = 67890,
            SubAccountId = 1,
            TransId = 987654321
        };

        customerService.IsBelonging(request.DownlineId, request.ViewerId, request.SubAccountId, request.TransId)
            .Returns(true);

        // Act
        var response = await grpcService.GetBelongingInfo(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsBelonging);
    }

    [Fact]
    public async Task GetAllProducts_WithValidRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var request = new GetAllProductsRequest
        {
            SiteName = "TestSite",
            Currency = 1
        };

        var products = new[]
        {
            new Alpha.Customer.API.Models.ProductModel { Id = 1, Code = "PROD1", Name = "Product 1" },
            new Alpha.Customer.API.Models.ProductModel { Id = 2, Code = "PROD2", Name = "Product 2" }
        };

        customerService.GetAllProducts(request.SiteName, request.Currency)
            .Returns(products);

        // Act
        var response = await grpcService.GetAllProducts(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Products);
        Assert.Equal(2, response.Products.Count);
        Assert.Equal("PROD1", response.Products[0].Code);
        Assert.Equal("PROD2", response.Products[1].Code);
    }

    [Fact]
    public async Task ClearProductsCache_WithValidRequest_CallsServiceCorrectly()
    {
        // Arrange
        var request = new ClearProductsCacheRequest
        {
            SiteName = "TestSite",
            Currency = 1
        };

        // Act
        var response = await grpcService.ClearProductsCache(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Result);
        await customerService.Received(1).ClearProductsCache(request.SiteName, request.Currency);
    }

    [Fact]
    public async Task GetCustomerStatus_WithNullCustomerStatus_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new GetCustomerStatusRequest
        {
            CustId = 99999,
            SubAccId = 0
        };

        customerService.GetCustomerStatus(Arg.Any<CustomerStatusParam>())
            .Returns((CustomerStatus)null);

        // Act
        var response = await grpcService.GetCustomerStatus(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(0, response.CustId);
        Assert.Equal(0, response.SubAccId);
        Assert.False(response.IsActive);
    }

    [Fact]
    public async Task GetDownlineInfo_WithNullDownlineInfo_ReturnsFalse()
    {
        // Arrange
        var request = new GetDownlineInfoRequest
        {
            ViewerId = 12345,
            DownlineUsername = "nonexistent"
        };

        customerService.GetDownlineInfo(request.ViewerId, request.DownlineUsername)
            .Returns((DownlineInfo)null);

        // Act
        var response = await grpcService.GetDownlineInfo(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsBelonging);
    }

    [Fact]
    public async Task GetDownlineInfo_WithZeroCustId_ReturnsFalse()
    {
        // Arrange
        var request = new GetDownlineInfoRequest
        {
            ViewerId = 12345,
            DownlineUsername = "zerocustuser"
        };

        var downlineInfo = new DownlineInfo(0, 3, false); // CustId = 0
        customerService.GetDownlineInfo(request.ViewerId, request.DownlineUsername)
            .Returns(downlineInfo);

        // Act
        var response = await grpcService.GetDownlineInfo(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsBelonging);
    }

    [Fact]
    public async Task GetBelongingInfo_WithFalseResult_ReturnsFalse()
    {
        // Arrange
        var request = new GetBeLongingInfoRequest
        {
            DownlineId = 12345,
            ViewerId = 67890,
            SubAccountId = 1,
            TransId = 987654321
        };

        customerService.IsBelonging(request.DownlineId, request.ViewerId, request.SubAccountId, request.TransId)
            .Returns(false);

        // Act
        var response = await grpcService.GetBelongingInfo(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsBelonging);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(grpcService);
    }
}