using Alpha.Reports.API.Criteria;
using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetOutstandingCriteriaTests
{
    [Theory]
    [InlineData(UserLevelModel.Super, "Age_Rpt_Outstanding_Master")]
    [InlineData(UserLevelModel.Master, "Age_Rpt_Outstanding_Agent")]
    [InlineData(UserLevelModel.Agent, "Age_Rpt_Outstanding_Member")]
    public void GetSettingKey_WhenViewAgentLevel_ReturnsValidSettingKey(byte level, string expectedSettingKey)
    {
        // Arrange
        var criteria = new GetOutstandingCriteria(level, 0, string.Empty, 0);

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Theory]
    [InlineData(UserLevelModel.Super)]
    [InlineData(UserLevelModel.Master)]
    [InlineData(UserLevelModel.Agent)]
    public void IsValid_WhenLevelIsNotMember_ReturnsValid(byte level)
    {
        // Arrange
        var criteria = new GetOutstandingCriteria(level, 0, string.Empty, 0);
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenLevelIsMember_ReturnsInvalid()
    {
        // Arrange
        var criteria = new GetOutstandingCriteria(UserLevelModel.Member, 0, string.Empty, 0);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
