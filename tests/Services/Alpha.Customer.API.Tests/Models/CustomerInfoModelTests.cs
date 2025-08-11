using Alpha.Customer.API.Models;
using Xunit;

namespace Alpha.Customer.API.Tests.Models;

public class CustomerInfoModelTests
{
    [Fact]
    public void CustomerInfoModel_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var customerInfo = new CustomerInfoModel(
            CustId: 12345,
            RoleId: 2,
            Site: "TestSite",
            Currency: 1
        );

        // Assert
        Assert.Equal(12345, customerInfo.CustId);
        Assert.Equal(2, customerInfo.RoleId);
        Assert.Equal("TestSite", customerInfo.Site);
        Assert.Equal(1, customerInfo.Currency);
    }

    [Theory]
    [InlineData(1, 1, "Site1", 0)]
    [InlineData(999999, 255, "LongSiteName", 999)]
    [InlineData(0, 0, "", -1)]
    public void CustomerInfoModel_WorksWithVariousValues(int custId, byte roleId, string site, int currency)
    {
        // Arrange & Act
        var customerInfo = new CustomerInfoModel(custId, roleId, site, currency);

        // Assert
        Assert.Equal(custId, customerInfo.CustId);
        Assert.Equal(roleId, customerInfo.RoleId);
        Assert.Equal(site, customerInfo.Site);
        Assert.Equal(currency, customerInfo.Currency);
    }

    [Fact]
    public void CustomerInfoModel_SupportsRecordEquality()
    {
        // Arrange
        var customerInfo1 = new CustomerInfoModel(123, 2, "Site", 1);
        var customerInfo2 = new CustomerInfoModel(123, 2, "Site", 1);
        var customerInfo3 = new CustomerInfoModel(124, 2, "Site", 1);

        // Act & Assert
        Assert.Equal(customerInfo1, customerInfo2);
        Assert.NotEqual(customerInfo1, customerInfo3);
        Assert.True(customerInfo1 == customerInfo2);
        Assert.False(customerInfo1 == customerInfo3);
    }
}
