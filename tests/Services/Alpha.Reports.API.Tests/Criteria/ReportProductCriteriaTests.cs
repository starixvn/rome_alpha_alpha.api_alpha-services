using Alpha.Reports.API.Criteria;

namespace Alpha.Reports.API.Tests.Criteria;

public class ReportProductCriteriaTests
{
    [Fact]
    public void GetSettingKey_Always_ThrowNotImplementedException()
    {
        // Arrange
        var criteria = new ReportProductCriteria(0, DateTime.Now, DateTime.Now, Enumerable.Empty<int>());

        // Act
        // Assert
        Assert.Throws<NotImplementedException>(() => criteria.GetSettingKey());
    }

    [Fact]
    public void IsValid_WhenCustIdIsZero_ReturnsInvalid()
    {
        // Arrange
        var custId = 0;
        var criteria = new ReportProductCriteria(custId, DateTime.Now, DateTime.Now, Enumerable.Empty<int>());
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenCustIdIsNegative_ReturnsInvalid()
    {
        // Arrange
        var custId = -1;
        var criteria = new ReportProductCriteria(custId, DateTime.Now, DateTime.Now, Enumerable.Empty<int>());
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenProductIdsIsEmpty_ReturnsInvalid()
    {
        // Arrange
        var criteria = new ReportProductCriteria(1, DateTime.Now, DateTime.Now, Enumerable.Empty<int>());
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenCustIdIsPositiveAndProductIdsHaveData_ReturnsValid()
    {
        // Arrange
        var custId = 1;
        var productIds = new List<int> { 1, 2 };
        var criteria = new ReportProductCriteria(custId, DateTime.Now, DateTime.Now, productIds);
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
