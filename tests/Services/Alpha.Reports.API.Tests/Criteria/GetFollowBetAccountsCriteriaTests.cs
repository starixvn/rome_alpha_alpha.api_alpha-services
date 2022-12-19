using Alpha.Reports.API.Criteria;

namespace Alpha.Reports.API.Tests.Criteria;

public class GetFollowBetAccountsCriteriaTests
{
    [Fact]
    public void GetSettingKey_Always_ReturnsValidSettingKey()
    {
        // Arrange
        var agentId = 0;
        var criteria = new GetFollowBetAccountsCriteria(agentId);
        var expectedSettingKey = "Age_Customer_FollowBetAccountListSelByAgent";

        // Act
        var actualSettingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expectedSettingKey, actualSettingKey);
    }

    [Fact]
    public void IsValid_WhenAgentIdIsZero_ReturnsInvalid()
    {
        // Arrange
        var agentId = 0;
        var criteria = new GetFollowBetAccountsCriteria(agentId);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenAgentIdIsNegative_ReturnsInvalid()
    {
        // Arrange
        var agentId = -1;
        var criteria = new GetFollowBetAccountsCriteria(agentId);
        var expectedIsValid = false;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }

    [Fact]
    public void IsValid_WhenAgentIdIsPositive_ReturnsValid()
    {
        // Arrange
        var agentId = 1;
        var criteria = new GetFollowBetAccountsCriteria(agentId);
        var expectedIsValid = true;

        // Act
        var actualIsValid = criteria.IsValid();

        // Assert
        Assert.Equal(expectedIsValid, actualIsValid);
    }
}
