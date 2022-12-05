namespace Alpha.Utilities.Models;

public static class UserLevelModel
{
    public const byte Super = 4;
    public const byte Master = 3;
    public const byte Agent = 2;
    public const byte Member = 1;

    public static bool IsMasterLevel(byte level) => level == Master;

    public static bool IsAgentLevel(byte level) => level == Agent;

    public static bool IsMemberLevel(byte level) => level == Member;

    public static string GetLevelName(byte level)
        => level switch
        {
            Super => "Super",
            Master => "Master",
            Agent => "Agent",
            _ => "Member",
        };

    public static string GetDirectDownlineLevelName(byte level)
        => level switch
        {
            Super => "Master",
            Master => "Agent",
            Agent => "Member",
            _ => string.Empty,
        };

    public static bool IsUpline(byte uplineLevelValue, byte downlineLevelValue)
        => GapBetweenTwoLevels(uplineLevelValue, downlineLevelValue) >= 1;

    private static int GapBetweenTwoLevels(byte uplineLevelValue, byte downlineLevelValue)
        => uplineLevelValue - downlineLevelValue;
}