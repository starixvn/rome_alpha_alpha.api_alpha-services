using Alpha.Shared.Services.Criteria;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetBalanceUpDayAndMoveBetsDayCriteriaTests
{
    [Fact]
    public void GetSettingKey_Always_ReturnsValidSettingKey()
    {
        // Arrange
        var criteria = new GetBalanceUpDayAndMoveBetsDayCriteria();
        var expectedSettingKey = "Age_SelBalanceUpDay";

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Fact]
    public void IsValid_Always_ReturnsValid()
    {
        // Arrange
        var criteria = new GetBalanceUpDayAndMoveBetsDayCriteria();
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
