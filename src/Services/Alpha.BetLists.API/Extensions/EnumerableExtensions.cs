namespace Alpha.BetLists.API.Extensions;

public static class EnumerableExtensions
{
    public static string JoinWithComma<T>(this IEnumerable<T> source) => string.Join(",", source);
}