using Alpha.Reports.API.Criteria;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetProductsDetailCriteriaTests
{
    [Fact]
    public void GetSettingKey_Always_ReturnsValidSettingKey()
    {
        // Arrange
        var criteria = new GetProductsDetailCriteria();
        var expectedSettingKey = "Age_Rpt_GetProductDetail";

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Fact]
    public void IsValid_Always_ReturnsValid()
    {
        // Arrange
        var criteria = new GetProductsDetailCriteria();
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
