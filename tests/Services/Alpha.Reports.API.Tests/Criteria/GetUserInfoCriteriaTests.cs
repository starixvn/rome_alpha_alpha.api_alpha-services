using Alpha.Reports.API.Criteria;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetUserInfoCriteriaTests
{
    [Fact]
    public void GetSettingKey_Always_ReturnsValidSettingKey()
    {
        // Arrange
        var criteria = new GetUserInfoCriteria(0, 0);
        var expectedSettingKey = "Age_CustomerSelUserInfo";

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Fact]
    public void IsValid_WhenCustIdIsZero_ReturnsInvalid()
    {
        // Arrange
        var custId = 0;
        var criteria = new GetUserInfoCriteria(custId, 0);
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
        var criteria = new GetUserInfoCriteria(custId, 0);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenCustIdIsPositive_ReturnsValid()
    {
        // Arrange
        var custId = 1;
        var criteria = new GetUserInfoCriteria(custId, 0);
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
