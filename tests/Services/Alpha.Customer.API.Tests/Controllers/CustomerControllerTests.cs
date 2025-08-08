using System.Threading.Tasks;
using Alpha.Customer.API.Controllers;
using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;
using Alpha.Customer.API.Services;
using NSubstitute;

namespace Alpha.Customer.API.Tests.Controllers;

public class CustomerControllerTests
{
    private readonly ICustomerService customerService = Substitute.For<ICustomerService>();
    private readonly CustomerController controller;

    public CustomerControllerTests()
    {
        controller = new CustomerController(customerService);
    }

    [Fact]
    public async Task GetCustomerStatus_WithValidParameters_ReturnsCustomerStatus()
    {
        // Arrange
        var custId = 12345;
        var subAccId = 0;
        var expectedStatus = new CustomerStatus(custId, subAccId, true, false, false, "[E][H]");

        customerService.GetCustomerStatus(Arg.Any<CustomerStatusParam>())
            .Returns(expectedStatus);

        // Act
        var result = await controller.GetCustomerStatus(custId, subAccId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(custId, result.CustId);
        Assert.Equal(subAccId, result.SubAccId);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetCustomerStatus_CallsServiceWithCorrectParam()
    {
        // Arrange
        var custId = 12345;
        var subAccId = 999;
        var customerStatus = new CustomerStatus(custId, subAccId, true, false, false, "[E][H]");

        customerService.GetCustomerStatus(Arg.Any<CustomerStatusParam>())
            .Returns(customerStatus);

        // Act
        await controller.GetCustomerStatus(custId, subAccId);

        // Assert
        await customerService.Received(1).GetCustomerStatus(
            Arg.Is<CustomerStatusParam>(p => p.CustId == custId && p.SubAccId == subAccId));
    }

    [Fact]
    public async Task GetDownlineInfo_WithValidParameters_ReturnsDownlineInfo()
    {
        // Arrange
        var custId = 12345;
        var downlineUsername = "testuser";
        var expectedDownline = new DownlineInfo(67890, 3, false);

        customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
            .Returns(expectedDownline);

        // Act
        var result = await controller.GetDownlineInfo(custId, downlineUsername);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(67890, result.CustId);
        Assert.Equal(3, result.RoleId);
        Assert.False(result.IsSubAcc);
    }

    [Fact]
    public async Task GetDownlineInfo_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var custId = 12345;
        var downlineUsername = "testuser";
        var downlineInfo = new DownlineInfo(67890, 3, false);

        customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
            .Returns(downlineInfo);

        // Act
        await controller.GetDownlineInfo(custId, downlineUsername);

        // Assert
        await customerService.Received(1).GetDownlineInfo(custId, downlineUsername);
    }

    [Fact]
    public async Task GetDownlineInfo_WithNullResult_ReturnsNull()
    {
        // Arrange
        var custId = 12345;
        var downlineUsername = "nonexistentuser";

        customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
            .Returns((DownlineInfo)null);

        // Act
        var result = await controller.GetDownlineInfo(custId, downlineUsername);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCustomerStatus_WithNegativeCustId_StillCallsService()
    {
        // Arrange
        var custId = -1;
        var subAccId = 0;
        var customerStatus = new CustomerStatus(custId, subAccId, false, false, false, "");

        customerService.GetCustomerStatus(Arg.Any<CustomerStatusParam>())
            .Returns(customerStatus);

        // Act
        var result = await controller.GetCustomerStatus(custId, subAccId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(custId, result.CustId);
        await customerService.Received(1).GetCustomerStatus(
            Arg.Is<CustomerStatusParam>(p => p.CustId == custId && p.SubAccId == subAccId));
    }

    [Fact]
    public async Task GetCustomerStatus_WithLargeSubAccId_HandlesCorrectly()
    {
        // Arrange
        var custId = 12345;
        var subAccId = int.MaxValue;
        var customerStatus = new CustomerStatus(custId, subAccId, true, false, false, "[E][H]");

        customerService.GetCustomerStatus(Arg.Any<CustomerStatusParam>())
            .Returns(customerStatus);

        // Act
        var result = await controller.GetCustomerStatus(custId, subAccId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subAccId, result.SubAccId);
    }

    [Fact]
    public async Task GetDownlineInfo_WithEmptyUsername_StillCallsService()
    {
        // Arrange
        var custId = 12345;
        var downlineUsername = "";
        var downlineInfo = new DownlineInfo(0, 0, false);

        customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
            .Returns(downlineInfo);

        // Act
        var result = await controller.GetDownlineInfo(custId, downlineUsername);

        // Assert
        Assert.NotNull(result);
        await customerService.Received(1).GetDownlineInfo(custId, downlineUsername);
    }

    [Fact]
    public async Task GetDownlineInfo_WithSpecialCharactersInUsername_HandlesCorrectly()
    {
        // Arrange
        var custId = 12345;
        var downlineUsername = "test@user!#$%";
        var downlineInfo = new DownlineInfo(67890, 3, true);

        customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
            .Returns(downlineInfo);

        // Act
        var result = await controller.GetDownlineInfo(custId, downlineUsername);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSubAcc);
        await customerService.Received(1).GetDownlineInfo(custId, downlineUsername);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(controller);
    }
}
