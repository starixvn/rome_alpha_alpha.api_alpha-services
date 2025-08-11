using Alpha.ApiKey.API.Models;
using Xunit;

namespace Alpha.ApiKey.API.Tests.Models;

public class ResultTests
{
    [Fact]
    public void Result_CanBeCreated_WithCodeAndMessage()
    {
        // Arrange & Act
        var result = new Result(0, "Success");

        // Assert
        Assert.Equal(0, result.Code);
        Assert.Equal("Success", result.Message);
    }

    [Theory]
    [InlineData(0, false)] // Success
    [InlineData(1, true)]  // Error
    [InlineData(2, true)]  // Error
    [InlineData(-1, false)] // Negative codes are considered success
    public void IsFailed_ReturnsCorrectValue_BasedOnCode(int code, bool expectedIsFailed)
    {
        // Arrange
        var result = new Result(code, "Test message");

        // Act
        var isFailed = result.IsFailed;

        // Assert
        Assert.Equal(expectedIsFailed, isFailed);
    }

    [Fact]
    public void Result_SupportsRecordEquality()
    {
        // Arrange
        var result1 = new Result(1, "Error");
        var result2 = new Result(1, "Error");
        var result3 = new Result(2, "Error");

        // Act & Assert
        Assert.Equal(result1, result2);
        Assert.NotEqual(result1, result3);
        Assert.True(result1 == result2);
        Assert.False(result1 == result3);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "Validation error")]
    [InlineData(500, "Internal server error")]
    [InlineData(-1, "Negative code message")]
    public void Result_WorksWithVariousValues(int code, string message)
    {
        // Arrange & Act
        var result = new Result(code, message);

        // Assert
        Assert.Equal(code, result.Code);
        Assert.Equal(message, result.Message);
        Assert.Equal(code > 0, result.IsFailed);
    }
}
