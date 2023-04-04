namespace Alpha.Utilities.Models;

public static class SystemTimeModel
{
    private static DateTime? today;

    public static DateTime Today
    {
        get { return today ?? DateTime.Today; }
        set { today = value; }
    }
}