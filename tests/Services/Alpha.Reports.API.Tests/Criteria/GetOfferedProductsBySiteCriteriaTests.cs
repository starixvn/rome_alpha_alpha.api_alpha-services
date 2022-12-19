using Alpha.Reports.API.Criteria;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetOfferedProductsBySiteCriteriaTests
{
    [Fact]
    public void GetSettingKey_Always_ReturnsValidSettingKey()
    {
        // Arrange
        var criteria = new GetOfferedProductsBySiteCriteria(0, string.Empty, 0, 0);
        var expectedSettingKey = "Acc_CustSetting_CheckMacauEnable";

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValid_WhenSiteIsNullOrEmpty_ReturnsInvalid(string site)
    {
        // Arrange
        var criteria = new GetOfferedProductsBySiteCriteria(0, site, 0, 0);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenCurrencyIdIsZero_ReturnsInvalid()
    {
        // Arrange
        int currencyId = 0;
        var criteria = new GetOfferedProductsBySiteCriteria(0, "site", currencyId, 0);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenCurrencyIdIsNegative_ReturnsInvalid()
    {
        // Arrange
        int currencyId = -1;
        var criteria = new GetOfferedProductsBySiteCriteria(0, "site", currencyId, 0);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenCurrencyIdIsPositive_ReturnsValid()
    {
        // Arrange
        int currencyId = 1;
        var criteria = new GetOfferedProductsBySiteCriteria(0, "site", currencyId, 0);
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
