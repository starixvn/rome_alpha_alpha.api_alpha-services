using Alpha.ApiKey.API.Models;
using Xunit;

namespace Alpha.ApiKey.API.Tests.Models;

public class VerifyApiKeyResultTests
{
    [Fact]
    public void VerifyApiKeyResult_CanBeCreated_WithRequiredParameters()
    {
        // Arrange & Act
        var result = new VerifyApiKeyResult(true, "");

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("", result.ErrorMessage);
        Assert.Equal(0, result.CustId);
        Assert.Equal(0, result.RoleId);
        Assert.Null(result.Scopes);
    }

    [Fact]
    public void VerifyApiKeyResult_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var result = new VerifyApiKeyResult(true, "No error")
        {
            CustId = 12345,
            RoleId = 2,
            Scopes = "read,write"
        };

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("No error", result.ErrorMessage);
        Assert.Equal(12345, result.CustId);
        Assert.Equal(2, result.RoleId);
        Assert.Equal("read,write", result.Scopes);
    }

    [Theory]
    [InlineData(true, "")]
    [InlineData(false, "Invalid API key")]
    [InlineData(true, "Warning message")]
    [InlineData(false, "Authentication failed")]
    public void VerifyApiKeyResult_WorksWithVariousAuthenticationStates(bool isAuthenticated, string errorMessage)
    {
        // Arrange & Act
        var result = new VerifyApiKeyResult(isAuthenticated, errorMessage);

        // Assert
        Assert.Equal(isAuthenticated, result.IsAuthenticated);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void VerifyApiKeyResult_SupportsRecordEquality()
    {
        // Arrange
        var result1 = new VerifyApiKeyResult(true, "Success") { CustId = 123, RoleId = 1, Scopes = "read" };
        var result2 = new VerifyApiKeyResult(true, "Success") { CustId = 123, RoleId = 1, Scopes = "read" };
        var result3 = new VerifyApiKeyResult(false, "Failed") { CustId = 123, RoleId = 1, Scopes = "read" };

        // Act & Assert
        Assert.Equal(result1, result2);
        Assert.NotEqual(result1, result3);
        Assert.True(result1 == result2);
        Assert.False(result1 == result3);
    }

    [Fact]
    public void VerifyApiKeyResult_DefaultErrorMessage_IsEmpty()
    {
        // Arrange & Act
        var result = new VerifyApiKeyResult(true);

        // Assert
        Assert.Equal("", result.ErrorMessage);
    }

    [Theory]
    [InlineData(1, 1, "admin")]
    [InlineData(999999, 255, "read,write,delete")]
    [InlineData(0, 0, "")]
    [InlineData(-1, -1, null)]
    public void VerifyApiKeyResult_WorksWithVariousCustomerData(int custId, int roleId, string scopes)
    {
        // Arrange & Act
        var result = new VerifyApiKeyResult(true, "Test")
        {
            CustId = custId,
            RoleId = roleId,
            Scopes = scopes
        };

        // Assert
        Assert.Equal(custId, result.CustId);
        Assert.Equal(roleId, result.RoleId);
        Assert.Equal(scopes, result.Scopes);
    }
}
