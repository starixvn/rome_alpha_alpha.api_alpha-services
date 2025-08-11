using Alpha.ApiKey.API.Models;
using Xunit;

namespace Alpha.ApiKey.API.Tests.Models;

public class VerifyExtApiKeyResultTests
{
    [Fact]
    public void VerifyExtApiKeyResult_CanBeCreated_WithDefaultValues()
    {
        // Arrange & Act
        var result = new VerifyExtApiKeyResult();

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal(0, result.CustId);
        Assert.Equal(0, result.SubAccId);
        Assert.Null(result.ErrMsg);
        Assert.Null(result.Site);
        Assert.Equal(0, result.Currency);
    }

    [Fact]
    public void VerifyExtApiKeyResult_CanBeCreated_WithSetProperties()
    {
        // Arrange & Act
        var result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = true,
            CustId = 12345,
            SubAccId = 67890,
            ErrMsg = "No error",
            Site = "TestSite",
            Currency = 1
        };

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal(12345, result.CustId);
        Assert.Equal(67890, result.SubAccId);
        Assert.Equal("No error", result.ErrMsg);
        Assert.Equal("TestSite", result.Site);
        Assert.Equal(1, result.Currency);
    }

    [Theory]
    [InlineData(true, 1, 0, "", "Site1", 1)]
    [InlineData(false, 999999, 12345, "Authentication failed", "LongSiteName", 999)]
    [InlineData(true, 0, 0, null, null, 0)]
    [InlineData(false, -1, -1, "Invalid key", "", -1)]
    public void VerifyExtApiKeyResult_WorksWithVariousValues(
        bool isAuthenticated, int custId, int subAccId, string errMsg, string site, int currency)
    {
        // Arrange & Act
        var result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = isAuthenticated,
            CustId = custId,
            SubAccId = subAccId,
            ErrMsg = errMsg,
            Site = site,
            Currency = currency
        };

        // Assert
        Assert.Equal(isAuthenticated, result.IsAuthenticated);
        Assert.Equal(custId, result.CustId);
        Assert.Equal(subAccId, result.SubAccId);
        Assert.Equal(errMsg, result.ErrMsg);
        Assert.Equal(site, result.Site);
        Assert.Equal(currency, result.Currency);
    }

    [Fact]
    public void VerifyExtApiKeyResult_PropertiesCanBeModified()
    {
        // Arrange
        var result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = false,
            CustId = 1,
            SubAccId = 1,
            ErrMsg = "Original",
            Site = "Original",
            Currency = 0
        };

        // Act
        result.IsAuthenticated = true;
        result.CustId = 2;
        result.SubAccId = 2;
        result.ErrMsg = "Modified";
        result.Site = "Modified";
        result.Currency = 1;

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal(2, result.CustId);
        Assert.Equal(2, result.SubAccId);
        Assert.Equal("Modified", result.ErrMsg);
        Assert.Equal("Modified", result.Site);
        Assert.Equal(1, result.Currency);
    }

    [Fact]
    public void VerifyExtApiKeyResult_SupportsSuccessfulAuthentication()
    {
        // Arrange & Act
        var result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = true,
            CustId = 123,
            SubAccId = 0,
            ErrMsg = null,
            Site = "MainSite",
            Currency = 1
        };

        // Assert - Successful authentication scenario
        Assert.True(result.IsAuthenticated);
        Assert.True(result.CustId > 0);
        Assert.Null(result.ErrMsg);
        Assert.NotNull(result.Site);
    }

    [Fact]
    public void VerifyExtApiKeyResult_SupportsFailedAuthentication()
    {
        // Arrange & Act
        var result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = false,
            CustId = 0,
            SubAccId = 0,
            ErrMsg = "Invalid API key provided",
            Site = null,
            Currency = 0
        };

        // Assert - Failed authentication scenario
        Assert.False(result.IsAuthenticated);
        Assert.Equal(0, result.CustId);
        Assert.NotNull(result.ErrMsg);
        Assert.Contains("Invalid", result.ErrMsg);
    }
}
