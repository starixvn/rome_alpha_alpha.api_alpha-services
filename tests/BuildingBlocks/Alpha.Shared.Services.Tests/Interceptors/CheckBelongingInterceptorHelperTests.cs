using Alpha.Shared.Services.Contracts;
using Alpha.Shared.Services.Interceptors;
using Grpc.Core;
using NSubstitute;
using Xunit;

namespace Alpha.Shared.Services.Tests.Interceptors;

public class CheckBelongingInterceptorHelperTests
{
    [Fact]
    public async Task PerformBelongingCheck_WithValidRequest_CallsGetDownlineInfoAsync()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = "testuser" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = true });

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, "testuser");
    }

    [Fact]
    public async Task PerformBelongingCheck_WithBelongingFalse_ReturnsFalse()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = "testuser" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = false });

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.False(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, "testuser");
    }

    [Fact]
    public async Task PerformBelongingCheck_WithNullRequest_ReturnsTrue()
    {
        // Arrange
        TestRequest? request = null;
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.DidNotReceive().Invoke(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact]
    public async Task PerformBelongingCheck_WithEmptyRequestString_ReturnsTrue()
    {
        // Arrange
        var request = new EmptyRequest();
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.DidNotReceive().Invoke(Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CheckBelongingWithErrorHandling_WithBelongingTrue_ReturnsTrue()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = "testuser" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        var handleForbiddenResponse = Substitute.For<Func<string, Task>>();
        
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = true });

        // Act
        var result = await CheckBelongingInterceptorHelper.CheckBelongingWithErrorHandling(
            request, getDownlineInfoAsync, handleForbiddenResponse);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, "testuser");
        await handleForbiddenResponse.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task CheckBelongingWithErrorHandling_WithBelongingFalse_CallsErrorHandlerAndReturnsFalse()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = "testuser" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        var handleForbiddenResponse = Substitute.For<Func<string, Task>>();
        
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = false });

        // Act
        var result = await CheckBelongingInterceptorHelper.CheckBelongingWithErrorHandling(
            request, getDownlineInfoAsync, handleForbiddenResponse);

        // Assert
        Assert.False(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, "testuser");
        await handleForbiddenResponse.Received(1).Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task CheckBelongingWithErrorHandling_WithException_CallsErrorHandlerAndReturnsFalse()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = "testuser" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        var handleForbiddenResponse = Substitute.For<Func<string, Task>>();
        var expectedException = new InvalidOperationException("Test exception");
        
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Throws(expectedException);

        // Act
        var result = await CheckBelongingInterceptorHelper.CheckBelongingWithErrorHandling(
            request, getDownlineInfoAsync, handleForbiddenResponse);

        // Assert
        Assert.False(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, "testuser");
        await handleForbiddenResponse.Received(1).Invoke(Arg.Is<string>(s => s.Contains("Test exception")));
    }

    [Fact]
    public async Task CreateForbiddenResponse_WithValidContext_CompletesSuccessfully()
    {
        // Arrange
        var context = Substitute.For<ServerCallContext>();
        const string errorMessage = "Test error message";

        // Act & Assert
        await CheckBelongingInterceptorHelper.CreateForbiddenResponse(context, errorMessage);
        
        // The method should complete without throwing an exception
        Assert.True(true);
    }

    [Fact]
    public async Task PerformBelongingCheck_WithZeroViewerId_StillCallsService()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 0, CustName = "testuser" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = true });

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.Received(1).Invoke(0, "testuser");
    }

    [Fact]
    public async Task PerformBelongingCheck_WithEmptyCustName_StillCallsService()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = "" };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = true });

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, "");
    }

    [Fact]
    public async Task PerformBelongingCheck_WithNullCustName_StillCallsService()
    {
        // Arrange
        var request = new TestRequest { ViewerId = 123, CustName = null! };
        var getDownlineInfoAsync = Substitute.For<Func<int, string, Task<dynamic>>>();
        getDownlineInfoAsync.Invoke(Arg.Any<int>(), Arg.Any<string>())
            .Returns(new { IsBelonging = true });

        // Act
        var result = await CheckBelongingInterceptorHelper.PerformBelongingCheck(
            request, getDownlineInfoAsync);

        // Assert
        Assert.True(result);
        await getDownlineInfoAsync.Received(1).Invoke(123, null!);
    }
}

// Test helper classes
public class TestRequest : IBaseRequest
{
    public int ViewerId { get; set; }
    public string CustName { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"{{\"ViewerId\":{ViewerId},\"CustName\":\"{CustName}\"}}";
    }
}

public class EmptyRequest
{
    public override string ToString()
    {
        return "";
    }
}
