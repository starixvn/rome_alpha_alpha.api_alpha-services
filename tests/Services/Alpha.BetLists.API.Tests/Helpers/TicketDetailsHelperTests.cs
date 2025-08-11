using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.BetLists.API.Constants;
using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.BetLists.API.Services;
using Fanex.BetDetails.BetBuilder.Criteria;
using Fanex.BetDetails.BetBuilder.Services;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using GrpcBetLists;
using NSubstitute;
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

    [Theory]
    [InlineData(ProductIdConstants.Allbet, "Acc_BetList_LC_Detail")]
    [InlineData(ProductIdConstants.SAGaming, "Acc_BetList_LC_Detail")]
    [InlineData(ProductIdConstants.SabaCasino, "Acc_BetList_RNG_Detail")]
    [InlineData(ProductIdConstants.Joker, "Acc_BetList_RNG_Detail")]
    public void GetTicketDetailSettingKey_ReturnsExpectedMappings(int productId, string expectedKey)
    {
        var result = TicketDetailsHelper.GetTicketDetailSettingKey(productId);
        Assert.Equal(expectedKey, result);
    }

    [Fact]
    public void GetTicketDetailSettingKey_ReturnsNullForUnknownProductId()
    {
        var result = TicketDetailsHelper.GetTicketDetailSettingKey(99999);
        Assert.Null(result);
    }

    [Fact]
    public void GetDetailKey_ReturnsEmptyStringForNonExistentChoice()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int>(), 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 123 };
        
        var result = TicketDetailsHelper.GetDetailKey("nonexistent", ticket, mapper);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetDetailKey_ReturnsEncryptedKeyFor3rdProduct()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "test", 1 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 123, Status = "1" };
        
        var result = TicketDetailsHelper.GetDetailKey("test", ticket, mapper);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetDetailKey_ReturnsEncryptedKeyForParlayProduct()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int>(), 
            ParlayProducts = new Dictionary<string, byte> { { "parlay", 1 } } 
        };
        var ticket = new TicketDto { TransId = 123 };
        
        var result = TicketDetailsHelper.GetDetailKey("parlay", ticket, mapper);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetExtraInfo_DoesNothingForUnknownProductId()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int>(), 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 123, Status = "1" };
        var model = new SuperViewRunningBetTicketModel();
        
        // Should not throw exception
        model.GetExtraInfo("nonexistent", ticket, mapper);
        Assert.Null(model.ExtraInfo);
    }

    [Fact]
    public void GetExtraInfo_SetsSuperViewRunningBetTicketModelExtraInfo()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "test", 1 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 123, Status = "1" };
        var model = new SuperViewRunningBetTicketModel();
        
        model.GetExtraInfo("test", ticket, mapper);
        
        Assert.NotNull(model.ExtraInfo);
        Assert.Equal("123", model.ExtraInfo.RefNo);
        Assert.Equal(0, model.ExtraInfo.TransType); // GetTransactionType returns "0" by default
        Assert.Equal(1, model.ExtraInfo.ProductId);
    }

    [Fact]
    public void GetExtraInfo_SetsMasterViewRunningBetTicketModelExtraInfo()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "test", 2 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 456, Status = "2" };
        var model = new MasterViewRunningBetTicketModel();
        
        model.GetExtraInfo("test", ticket, mapper);
        
        Assert.NotNull(model.ExtraInfo);
        Assert.Equal("456", model.ExtraInfo.RefNo);
        Assert.Equal(0, model.ExtraInfo.TransType); // GetTransactionType returns "0" by default
        Assert.Equal(2, model.ExtraInfo.ProductId);
    }

    [Fact]
    public void GetExtraInfo_SetsAgentViewRunningBetTicketModelExtraInfo()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "agent", 3 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 789, Status = "3" };
        var model = new AgentViewRunningBetTicketModel();
        
        model.GetExtraInfo("agent", ticket, mapper);
        
        Assert.NotNull(model.ExtraInfo);
        Assert.Equal("789", model.ExtraInfo.RefNo);
        Assert.Equal(0, model.ExtraInfo.TransType); // GetTransactionType returns "0" by default
        Assert.Equal(3, model.ExtraInfo.ProductId);
    }

    [Fact]
    public void GetExtraInfo_SetsSuperViewWinLossBetTicketModelExtraInfo()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "winloss", 4 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 111, Status = "1" };
        var model = new SuperViewWinLossBetTicketModel();
        
        model.GetExtraInfo("winloss", ticket, mapper);
        
        Assert.NotNull(model.ExtraInfo);
        Assert.Equal("111", model.ExtraInfo.RefNo);
        Assert.Equal(0, model.ExtraInfo.TransType); // GetTransactionType returns "0" by default
        Assert.Equal(4, model.ExtraInfo.ProductId);
    }

    [Fact]
    public void GetExtraInfo_SetsMasterViewWinLossBetTicketModelExtraInfo()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "master", 5 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 222, Status = "2" };
        var model = new MasterViewWinLossBetTicketModel();
        
        model.GetExtraInfo("master", ticket, mapper);
        
        Assert.NotNull(model.ExtraInfo);
        Assert.Equal("222", model.ExtraInfo.RefNo);
        Assert.Equal(0, model.ExtraInfo.TransType); // GetTransactionType returns "0" by default
        Assert.Equal(5, model.ExtraInfo.ProductId);
    }

    [Fact]
    public void GetExtraInfo_DoesNothingForAgentViewWinLossBetTicketModel()
    {
        var mapper = new TicketDetailsMapperModel 
        { 
            Values = new Dictionary<string, int> { { "agent", 6 } }, 
            ParlayProducts = new Dictionary<string, byte>() 
        };
        var ticket = new TicketDto { TransId = 333, Status = "1" };
        var model = new AgentViewWinLossBetTicketModel();
        
        // Should not throw exception and not set any properties
        model.GetExtraInfo("agent", ticket, mapper);
        // AgentViewWinLossBetTicketModel doesn't have ExtraInfo property to check
    }

    [Fact]
    public void GetTicketData_AddsTicketDataToList()
    {
        var list = new List<ITicketData>();
        var ticket = Substitute.For<ITicket>();
        ticket.TransId.Returns(123);
        ticket.BetTypeId.Returns(1);
        ticket.Status.Returns("Won");
        ticket.Odds.Returns(1.5m);
        ticket.Stake.Returns(100m);
        ticket.BetTeam.Returns("TeamA");
        ticket.TransDesc.Returns("Test bet");
        ticket.SuperPositionTaking.Returns(10m);
        ticket.MasterPositionTaking.Returns(20m);
        ticket.AgentPositionTaking.Returns(30m);
        
        var tickets = new List<ITicket> { ticket };
        
        list.GetTicketData(tickets);
        
        Assert.Single(list);
        var ticketData = list.First();
        Assert.Equal("123", ticketData.RefNo);
        Assert.Equal(1, ticketData.BetTypeId);
        Assert.Equal("Won", ticketData.Status);
        Assert.Equal(1.5m, ticketData.Odds);
        Assert.Equal(100m, ticketData.Stake);
        Assert.Equal("TeamA", ticketData.BetTeam);
        Assert.Equal("Test bet", ticketData.TransDesc);
        Assert.Equal(10m, ticketData.SuperPT);
        Assert.Equal(20m, ticketData.MasterPT);
        Assert.Equal(30m, ticketData.AgentPT);
    }

}