using Alpha.Customer.API.Criteria;
using Alpha.Customer.API.DataAccesses;

namespace Alpha.Customer.API.Tests.DataAccesses;

public class CustomerDataAccessTests
{
    private readonly CustomerDataAccess dataAccess;

    public CustomerDataAccessTests()
    {
        dataAccess = new CustomerDataAccess();
    }

    [Fact]
    public async Task GetCustomerInfo_WithInvalidCriteria_ReturnsDefaultCustomerInfo()
    {
        // Arrange - Create criteria with invalid values that will fail IsValid() check
        var invalidCriteria = new GetCustomerInfoCriteria(0, "");

        // Act
        var result = await dataAccess.GetCustomerInfo(invalidCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.CustId);
        Assert.Equal(0, result.RoleId);
        Assert.Equal(string.Empty, result.Site);
        Assert.Equal(0, result.Currency);
    }

    [Fact(Skip = "Requires database connection - should be run as integration test")]
    public async Task GetCustomerInfo_WithValidCriteria_CallsDatabase()
    {
        // Arrange
        var criteria = new GetCustomerInfoCriteria(12345, "testuser");

        // Act
        var result = await dataAccess.GetCustomerInfo(criteria);

        // Assert
        Assert.NotNull(result);
        // Note: Without actual database setup, this will return default values
        // In a real test environment, you would mock the database or use integration tests
    }

    [Fact(Skip = "Requires database connection - should be run as integration test")]
    public async Task GetCustomerInfo_WithNullUsername_ReturnsDefaultCustomerInfo()
    {
        // Arrange - Create criteria with null username
        var invalidCriteria = new GetCustomerInfoCriteria(12345, null);

        // Act
        var result = await dataAccess.GetCustomerInfo(invalidCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.CustId);
        Assert.Equal(0, result.RoleId);
        Assert.Equal(string.Empty, result.Site);
        Assert.Equal(0, result.Currency);
    }

    [Fact(Skip = "Requires database connection - should be run as integration test")]
    public async Task GetCustomerInfo_WithEmptyUsername_ReturnsDefaultCustomerInfo()
    {
        // Arrange - Create criteria with empty username
        var invalidCriteria = new GetCustomerInfoCriteria(12345, "");

        // Act
        var result = await dataAccess.GetCustomerInfo(invalidCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.CustId);
        Assert.Equal(0, result.RoleId);
        Assert.Equal(string.Empty, result.Site);
        Assert.Equal(0, result.Currency);
    }

    [Fact(Skip = "Requires database connection - should be run as integration test")]
    public async Task GetCustomerInfo_WithNegativeCustId_ReturnsDefaultCustomerInfo()
    {
        // Arrange - Create criteria with negative customer ID
        var invalidCriteria = new GetCustomerInfoCriteria(-1, "testuser");

        // Act
        var result = await dataAccess.GetCustomerInfo(invalidCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.CustId);
        Assert.Equal(0, result.RoleId);
        Assert.Equal(string.Empty, result.Site);
        Assert.Equal(0, result.Currency);
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(dataAccess);
    }
}
