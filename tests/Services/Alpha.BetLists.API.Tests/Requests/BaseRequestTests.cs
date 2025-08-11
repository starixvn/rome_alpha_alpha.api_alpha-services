using Alpha.BetLists.API.Requests;
using Xunit;

namespace Alpha.BetLists.API.Tests.Requests;

public class BaseRequestTests
{
    [Fact]
    public void BaseRequest_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var request = new BaseRequest("TestCustomer", 123);

        // Assert
        Assert.Equal("TestCustomer", request.CustName);
        Assert.Equal(123, request.ViewerId);
    }

    [Theory]
    [InlineData("Customer1", 1)]
    [InlineData("", 0)]
    [InlineData(null, -1)]
    public void BaseRequest_WorksWithVariousValues(string custName, int viewerId)
    {
        // Arrange & Act
        var request = new BaseRequest(custName, viewerId);

        // Assert
        Assert.Equal(custName, request.CustName);
        Assert.Equal(viewerId, request.ViewerId);
    }

    [Fact]
    public void BaseRequest_SupportsRecordEquality()
    {
        // Arrange
        var request1 = new BaseRequest("Customer", 123);
        var request2 = new BaseRequest("Customer", 123);
        var request3 = new BaseRequest("Customer", 456);

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.NotEqual(request1, request3);
        Assert.True(request1 == request2);
        Assert.False(request1 == request3);
    }
}
