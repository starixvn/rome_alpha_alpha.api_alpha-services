using Alpha.Reports.API.Criteria;
using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetWinLossDetailCriteriaTests
{
    [Theory]
    [InlineData(UserLevelModel.Super, "Age_Rpt_API_WinLossDetail_Master")]
    [InlineData(UserLevelModel.Master, "Age_Rpt_API_WinLossDetail_Agent")]
    [InlineData(UserLevelModel.Agent, "Age_Rpt_API_WinLossDetail_Member")]
    public void GetSettingKey_Always_ReturnsValidSettingKey(byte custLevel, string expectedSettingKey)
    {
        // Arrange
        var now = DateTime.Now;
        var criteria = new GetWinLossDetailCriteria(0, now, now, Enumerable.Empty<int>(), custLevel, string.Empty, now, now, 0);

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Fact]
    public void IsValid_Always_ReturnsValid()
    {
        // Arrange
        var now = DateTime.Now;
        var criteria = new GetWinLossDetailCriteria(0, now, now, Enumerable.Empty<int>(), 0, string.Empty, now, now, 0);
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

}
