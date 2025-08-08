using Alpha.Customer.API.Criteria;
using Alpha.Customer.API.DataAccesses;
using NSubstitute;

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

    [Fact]
    public async Task GetCustomerInfo_WithBothInvalidValues_ReturnsDefaultCustomerInfo()
    {
        // Arrange - Create criteria with both invalid custId (<=0) and invalid username (null/empty)
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

    [Fact(Skip = "Requires database connection - valid criteria will attempt database access")]
    public async Task GetCustomerInfo_WithNullUsernameButValidCustId_CallsDatabase()
    {
        // Arrange - Create criteria with null username but valid custId (criteria is valid per IsValid() logic)
        var criteria = new GetCustomerInfoCriteria(12345, null);

        // Act
        var result = await dataAccess.GetCustomerInfo(criteria);

        // Assert
        Assert.NotNull(result);
        // This test requires database setup to work properly
    }

    [Fact(Skip = "Requires database connection - valid criteria will attempt database access")]
    public async Task GetCustomerInfo_WithEmptyUsernameButValidCustId_CallsDatabase()
    {
        // Arrange - Create criteria with empty username but valid custId (criteria is valid per IsValid() logic)
        var criteria = new GetCustomerInfoCriteria(12345, "");

        // Act
        var result = await dataAccess.GetCustomerInfo(criteria);

        // Assert
        Assert.NotNull(result);
        // This test requires database setup to work properly
    }

    [Fact(Skip = "Requires database connection - valid criteria will attempt database access")]
    public async Task GetCustomerInfo_WithNegativeCustIdButValidUsername_CallsDatabase()
    {
        // Arrange - Create criteria with negative customer ID but valid username (criteria is valid per IsValid() logic)
        var criteria = new GetCustomerInfoCriteria(-1, "testuser");

        // Act
        var result = await dataAccess.GetCustomerInfo(criteria);

        // Assert
        Assert.NotNull(result);
        // This test requires database setup to work properly
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(dataAccess);
    }

    // Additional unit tests for validation logic that can be tested without database
    [Fact]
    public void GetCustomerInfoCriteria_WithValidCustId_IsValid()
    {
        // Arrange
        var criteria = new GetCustomerInfoCriteria(12345, null);

        // Act & Assert
        Assert.True(criteria.IsValid());
    }

    [Fact]
    public void GetCustomerInfoCriteria_WithValidUsername_IsValid()
    {
        // Arrange
        var criteria = new GetCustomerInfoCriteria(0, "testuser");

        // Act & Assert
        Assert.True(criteria.IsValid());
    }

    [Fact]
    public void GetCustomerInfoCriteria_WithBothValid_IsValid()
    {
        // Arrange
        var criteria = new GetCustomerInfoCriteria(12345, "testuser");

        // Act & Assert
        Assert.True(criteria.IsValid());
    }

    [Fact]
    public void GetCustomerInfoCriteria_WithBothInvalid_IsNotValid()
    {
        // Arrange - Both custId <= 0 AND username is null/empty
        var criteria1 = new GetCustomerInfoCriteria(0, null);
        var criteria2 = new GetCustomerInfoCriteria(0, "");
        var criteria3 = new GetCustomerInfoCriteria(-1, null);
        var criteria4 = new GetCustomerInfoCriteria(-1, "");

        // Act & Assert
        Assert.False(criteria1.IsValid());
        Assert.False(criteria2.IsValid());
        Assert.False(criteria3.IsValid());
        Assert.False(criteria4.IsValid());
    }

    [Fact]
    public void GetCustomerInfoCriteria_GetSettingKey_ReturnsCorrectValue()
    {
        // Arrange
        var criteria = new GetCustomerInfoCriteria(12345, "testuser");

        // Act
        var settingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal("Age_ManageAPI_CustInfo", settingKey);
    }
}
