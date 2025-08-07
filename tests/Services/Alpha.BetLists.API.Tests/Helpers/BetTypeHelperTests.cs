using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Constants;

namespace Alpha.BetLists.API.Tests.Helpers;

public class BetTypeHelperTests
{
    [Theory]
    [InlineData(BetTypeIdConstants.JackpotContribution, true)]
    [InlineData(BetTypeIdConstants.JackpotContribution_Mobile, true)]
    [InlineData(BetTypeIdConstants.JackpotContribution_Tablet, true)]
    [InlineData(BetTypeIdConstants.PPJackpotContribution, true)]
    [InlineData(BetTypeIdConstants.SGJackpotContribution, true)]
    public void IsJackpotContribution_WithJackpotContributionBetTypes_ReturnsTrue(int betType, bool expected)
    {
        // Act
        var result = BetTypeHelper.IsJackpotContribution(betType);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    [InlineData(1)]
    [InlineData(100)]
    public void IsJackpotContribution_WithNonJackpotContributionBetTypes_ReturnsFalse(int betType)
    {
        // Act
        var result = BetTypeHelper.IsJackpotContribution(betType);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJackpotContribution_WithValidJackpotContributionBetType_ReturnsTrue()
    {
        // Arrange - Using the actual constant values
        var jackpotBetTypes = new[]
        {
            BetTypeIdConstants.JackpotContribution,
            BetTypeIdConstants.JackpotContribution_Mobile,
            BetTypeIdConstants.JackpotContribution_Tablet,
            BetTypeIdConstants.PPJackpotContribution,
            BetTypeIdConstants.SGJackpotContribution
        };

        // Act & Assert
        foreach (var betType in jackpotBetTypes)
        {
            var result = BetTypeHelper.IsJackpotContribution(betType);
            Assert.True(result, $"BetType {betType} should be identified as jackpot contribution");
        }
    }

    [Fact]
    public void IsJackpotContribution_WithRandomNonJackpotBetTypes_ReturnsFalse()
    {
        // Arrange - Random bet types that should not be jackpot contribution
        var nonJackpotBetTypes = new[] { 1, 2, 3, 5, 10, 50, 200, 500, 1000 };

        // Act & Assert
        foreach (var betType in nonJackpotBetTypes)
        {
            var result = BetTypeHelper.IsJackpotContribution(betType);
            Assert.False(result, $"BetType {betType} should not be identified as jackpot contribution");
        }
    }
}
