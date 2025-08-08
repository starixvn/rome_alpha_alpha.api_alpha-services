using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.Parlay;
using Alpha.Shared.Services.Models;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Tests.Fixtures;

public static class TestDataFixtures
{
    public static List<ProductModel> GetSampleProducts()
    {
        // Since ProductModel has protected setters, we need to create objects differently
        // For test purposes, we'll return an empty list or mock these separately
        return new List<ProductModel>();
    }

    public static List<ProductDetailModel> GetSampleProductDetails()
    {
        // Since ProductDetailModel has readonly properties, we'll return an empty list
        // Individual tests should mock these as needed
        return new List<ProductDetailModel>();
    }

    public static List<TicketDto> GetSampleTicketDtos()
    {
        return new List<TicketDto>
        {
            new TicketDto 
            { 
                TransId = 1001, 
                RefNo = "2001", 
                TransDate = new DateTime(2023, 1, 15, 10, 30, 0),
                IP = "192.168.1.100"
            },
            new TicketDto 
            { 
                TransId = 1002, 
                RefNo = "2002", 
                TransDate = new DateTime(2023, 1, 15, 11, 45, 0),
                IP = "192.168.1.101"
            },
            new TicketDto 
            { 
                TransId = 1003, 
                RefNo = "2003", 
                TransDate = new DateTime(2023, 1, 15, 14, 20, 0),
                IP = "192.168.1.102"
            }
        };
    }

    public static List<TicketData> GetSampleTicketData()
    {
        return new List<TicketData>
        {
            new TicketData { RefNo = "2001", Status = "Running", Odds = 1.85m },
            new TicketData { RefNo = "2002", Status = "Win", Odds = 2.10m },
            new TicketData { RefNo = "2003", Status = "Lose", Odds = 1.95m }
        };
    }

    public static List<ComboParlayDetail> GetSampleComboParlayDetails()
    {
        return new List<ComboParlayDetail>
        {
            new ComboParlayDetail(new DateTime(2023, 1, 15), 3001, 1, 1, "Win", 1.8m, 100.00m, 80m, 5m),
            new ComboParlayDetail(new DateTime(2023, 1, 16), 3002, 2, 2, "Lose", 2.0m, 250.00m, -250m, 12.5m)
        };
    }

    public static List<MatchDetail> GetSampleMatchDetails()
    {
        return new List<MatchDetail>
        {
            new MatchDetail(3001, 1, 2, false),
            new MatchDetail(3002, 3, 4, true)
        };
    }

    public static BalanceUpDayAndMoveBetsDayModel GetSampleReportDates()
    {
        return new BalanceUpDayAndMoveBetsDayModel(
            new DateTime(2023, 1, 15),
            new DateTime(2023, 1, 16));
    }

    public static BetListData GetSampleBetListData()
    {
        var ticketDtos = GetSampleTicketDtos();
        var ticketData = GetSampleTicketData().Cast<ITicketData>().ToList();
        var outputParams = new BetlistOutputParams(0, string.Empty, 1000);
        
        return new BetListData(ticketDtos, ticketData, outputParams);
    }

    public static PagedData<TicketDto> GetSamplePagedTicketData(int pageIndex = 1, int pageSize = 10)
    {
        var tickets = GetSampleTicketDtos();
        return new PagedData<TicketDto>(tickets, pageIndex, pageSize, tickets.Count);
    }

    public static Dictionary<string, string> GetSampleCountryData()
    {
        return new Dictionary<string, string>
        {
            { "192.168.1.100", "United States" },
            { "192.168.1.101", "United Kingdom" },
            { "192.168.1.102", "Canada" }
        };
    }
}
