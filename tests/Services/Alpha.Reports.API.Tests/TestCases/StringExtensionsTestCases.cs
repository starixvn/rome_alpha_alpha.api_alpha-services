namespace Alpha.Reports.API.Tests.TestCases;

public static class StringExtensionsTestCases
{
    private static readonly IEnumerable<object[]> stringContainsNonNumericCharactersCases = new List<object[]>()
        {
            new object[] { "1, abc, 3, d, 4", new List<int> { 1, 3, 4 } },
            new object[] { "1, 200, 343, 33, abc", new List<int> { 1, 200, 343, 33 } },
            new object[] { "1000000000000000000000, ab, 12", new List<int> { 12 } }
        };

    public static IEnumerable<object[]> StringContainsNonNumericCharactersCases => stringContainsNonNumericCharactersCases;
}