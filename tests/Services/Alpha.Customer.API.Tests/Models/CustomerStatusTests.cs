using Alpha.Customer.API.Models;
using Xunit;

namespace Alpha.Customer.API.Tests.Models;

public class CustomerStatusTests
{
    [Fact]
    public void IsSubAcc_WhenSubAccIdIsZero_ReturnsFalse()
    {
        // Arrange
        var customerStatus = new CustomerStatus(1, 0, true, false, false, "");

        // Act
        var result = customerStatus.IsSubAcc;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSubAcc_WhenSubAccIdIsNonZero_ReturnsTrue()
    {
        // Arrange
        var customerStatus = new CustomerStatus(1, 5, true, false, false, "");

        // Act
        var result = customerStatus.IsSubAcc;

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, "", true)] // Main account always has report permission
    [InlineData(0, "[A][B]", true)] // Main account always has report permission regardless of PageAccess
    [InlineData(5, "[E]", true)] // Sub account with reports permission
    [InlineData(5, "[A][E][B]", true)] // Sub account with reports permission among others
    [InlineData(5, "[H][E][F]", true)] // Sub account with reports permission among others
    [InlineData(5, "", false)] // Sub account without reports permission
    [InlineData(5, "[A][B]", false)] // Sub account without reports permission
    [InlineData(5, "[H][F]", false)] // Sub account without reports permission
    public void HasReportPermission_ReturnsExpectedResult(int subAccId, string pageAccess, bool expected)
    {
        // Arrange
        var customerStatus = new CustomerStatus(1, subAccId, true, false, false, pageAccess);

        // Act
        var result = customerStatus.HasReportPermission;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "", true)] // Main account always has member information permission
    [InlineData(0, "[A][B]", true)] // Main account always has member information permission regardless of PageAccess
    [InlineData(5, "[H]", true)] // Sub account with member information permission
    [InlineData(5, "[A][H][B]", true)] // Sub account with member information permission among others
    [InlineData(5, "[E][H][F]", true)] // Sub account with member information permission among others
    [InlineData(5, "", false)] // Sub account without member information permission
    [InlineData(5, "[A][B]", false)] // Sub account without member information permission
    [InlineData(5, "[E][F]", false)] // Sub account without member information permission
    public void HasMemberInformationPermission_ReturnsExpectedResult(int subAccId, string pageAccess, bool expected)
    {
        // Arrange
        var customerStatus = new CustomerStatus(1, subAccId, true, false, false, pageAccess);

        // Act
        var result = customerStatus.HasMemberInformationPermission;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CustomerStatus_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var customerStatus = new CustomerStatus(
            CustId: 123,
            SubAccId: 456,
            IsActive: true,
            IsProtectedMode: false,
            IsLocked: true,
            PageAccess: "[A][B][E]"
        );

        // Assert
        Assert.Equal(123, customerStatus.CustId);
        Assert.Equal(456, customerStatus.SubAccId);
        Assert.True(customerStatus.IsActive);
        Assert.False(customerStatus.IsProtectedMode);
        Assert.True(customerStatus.IsLocked);
        Assert.Equal("[A][B][E]", customerStatus.PageAccess);
    }
}
