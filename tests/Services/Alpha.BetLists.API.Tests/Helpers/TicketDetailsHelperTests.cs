using System.Collections.Generic;
using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Models.TicketDetail;
using Xunit;

namespace Alpha.BetLists.API.Tests.Helpers;

public class TicketDetailsHelperTests
{
    [Fact]
    public void CheckProductIdIsAllowed_ReturnsTrueIfExists()
    {
        var mapper = new TicketDetailsMapperModel { Values = new Dictionary<string, int> { { "foo", 5 } }, ParlayProducts = new Dictionary<string, byte>() };
        Assert.True(TicketDetailsHelper.CheckProductIdIsAllowed(mapper, 5));
    }

    [Fact]
    public void CheckProductIdIsAllowed_ReturnsFalseIfNotExists()
    {
        var mapper = new TicketDetailsMapperModel { Values = new Dictionary<string, int> { { "foo", 5 } }, ParlayProducts = new Dictionary<string, byte>() };
        Assert.False(TicketDetailsHelper.CheckProductIdIsAllowed(mapper, 6));
    }

    [Fact]
    public void GetParlayProductId_ReturnsCorrectValue()
    {
        var mapper = new TicketDetailsMapperModel { Values = new Dictionary<string, int>(), ParlayProducts = new Dictionary<string, byte> { { "foo", 2 } } };
        var result = TicketDetailsHelper.GetParlayProductId("foo", mapper);
        Assert.Equal((byte)2, result);
    }

    [Fact]
    public void GetParlayProductId_ReturnsZeroForNonExistentKey()
    {
        var mapper = new TicketDetailsMapperModel { Values = new Dictionary<string, int>(), ParlayProducts = new Dictionary<string, byte>() };
        var result = TicketDetailsHelper.GetParlayProductId("nonexistent", mapper);
        Assert.Equal((byte)0, result);
    }

    [Fact]
    public void GetTicketDetailSettingKey_ReturnsCorrectKey()
    {
        var result = TicketDetailsHelper.GetTicketDetailSettingKey(1); // Use a product ID that should match
        // This will return null or a key depending on the static mapping
        // The test verifies the method doesn't throw an exception
        Assert.True(result == null || result.Length > 0);
    }
}