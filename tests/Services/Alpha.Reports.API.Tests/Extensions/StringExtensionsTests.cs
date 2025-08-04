using Alpha.Utilities.Extensions;
using Alpha.Reports.API.Tests.TestCases;

namespace Alpha.Reports.API.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ToListInt_StringIsNull_ReturnEmptyListInt()
    {
        // Arrange
        string? s = null;

        // Act
        var expected = StringExtensions.ToListInt(s ?? string.Empty);

        // Assert
        Assert.Empty(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void ToListInt_StringIsEmptyOrWhiteSpace_ReturnEmptyListInt(string s)
    {
        // Act
        var expected = s.ToListInt();

        // Assert
        Assert.Empty(expected);
    }

    [Theory]
    [MemberData(nameof(StringExtensionsTestCases.StringContainsNonNumericCharactersCases), MemberType = typeof(StringExtensionsTestCases))]
    public void ToListInt_Always_IgnoreNonNumericCharacters(string s, List<int> ids)
    {
        // Arrange

        // Act
        var expected = s.ToListInt();

        // Assert
        Assert.Equal(expected, ids);
    }
}
