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

    private static readonly int[] HorseRacingBetType =
    {
            BetTypeIdConstants.VirtualHorseRacing_Win,
            BetTypeIdConstants.VirtualHorseRacing_Place,
            BetTypeIdConstants.VirtualHorseRacing_WinPlace
        };

    public static bool IsOutRight(int betType) => betType == BetTypeIdConstants.Outright;

    public static bool IsLeapGaming(int betType)
        => BetTypeIdConstants.LeapGamingMin <= betType && betType <= BetTypeIdConstants.LeapGamingMax;

    public static bool IsHappy5LinkedGames(int betType)
        => BetTypeIdConstants.Happy5LinkedGamesMin <= betType && betType <= BetTypeIdConstants.Happy5LinkedGamesMax;

    public static bool IsHorseRacing(int betType) => HorseRacingBetType.Contains(betType);

    public static bool IsJackpotContribution(int betType) => JackpotContributionBetType.Contains(betType);
}