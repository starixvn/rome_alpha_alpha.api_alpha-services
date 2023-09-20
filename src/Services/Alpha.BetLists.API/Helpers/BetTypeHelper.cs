using Alpha.BetLists.API.Constants;

namespace Alpha.BetLists.API.Helpers;

public static class BetTypeHelper
{
    private static readonly int[] JackpotContributionBetType =
        {
            BetTypeIdConstants.JackpotContribution,
            BetTypeIdConstants.JackpotContribution_Mobile,
            BetTypeIdConstants.JackpotContribution_Tablet,
            BetTypeIdConstants.PPJackpotContribution,
            BetTypeIdConstants.SGJackpotContribution
        };

    public static bool IsJackpotContribution(int betType) => JackpotContributionBetType.Contains(betType);
}