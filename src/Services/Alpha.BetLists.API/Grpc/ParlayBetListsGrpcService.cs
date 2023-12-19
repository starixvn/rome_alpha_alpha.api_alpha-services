using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.Parlay;
using Alpha.Utilities.Helpers;
using Alpha.Utilities.Models;
using Grpc.Core;
using Newtonsoft.Json;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    public override Task<ParlaySelectionsResponse> ParlaySelections(GetParlaySelectionsRequest request, ServerCallContext context)
    {
        var response = new ParlaySelectionsResponse();
        var key = AesService.Decrypt(request.DetailKey);

        if (string.IsNullOrEmpty(key))
        {
            return Task.FromResult(response);
        }

        var detailKeyModel = JsonConvert.DeserializeObject<DetailKeyModel<TicketDto>>(key);
        if (request.RefNo != detailKeyModel.RefNo)
        {
            return Task.FromResult(response);
        }

        switch (detailKeyModel.ParlayProductId)
        {
            case TicketDetailsHelper.SingleParlay:
                response.Selections = TicketDetailsHelper.GetSingleParlaySelections(detailKeyModel.ticket, betBuilderService).ToString();
                break;
            case TicketDetailsHelper.MixParlay:
                response.Selections = TicketDetailsHelper.GetMixParlaySelections(detailKeyModel.ticket, parlayService, ticketHelper, builder).ToString();
                break;
            default:
                response.Selections = string.Empty;
                break;
        }
        return Task.FromResult(response);

    }

    public override async Task<ParlayDetailResponse> ParlayDetail(GetParlayDetailRequest request, ServerCallContext context)
    { 
        var response = new ParlayDetailResponse();
        var key = AesService.Decrypt(request.DetailKey);
        if (string.IsNullOrEmpty(key))
        {
            return response;
        }

        var detailKeyModel = JsonConvert.DeserializeObject<DetailKeyModel<TicketDto>>(key);
        if (request.RefNo != detailKeyModel.RefNo)
        {
            return response;
        }

        var ticket = detailKeyModel.ticket;
        var comboParlayDetailData = await parlayService.GetComboParlayDetail(
            long.Parse(ticket.RefNo),
            int.Parse(ticket.CustId.ToString())
           );

        var comboParlayDetailViewModel = new ComboParlayDetailViewModel(comboParlayDetailData, ticketHelper);
        response.ParlayDetailItems.AddRange(comboParlayDetailViewModel.ComboParlayDetailWrapperGroups.Select(group =>
        {
            ParlayDetailItem detailItem = new ParlayDetailItem
            {
                Date = group.Key,
                SubTotal = comboParlayDetailViewModel.SubTotalWinLost(group)
            };
            detailItem.Events.AddRange(group.Select(item =>
            {
                ParlayEventModel eventModel = new ParlayEventModel
                {
                    Event = GetEventsCombination(item.Events),
                    Odds = comboParlayDetailData.IsParlayBoost ? GetParlayBoost(item.Odds, item.MasterOdds, item.BoostRate) : item.MasterOdds,
                    Stake = item.Stake,
                    Winloss = GetWinLoss(item.WinLost, item.Comm),
                    Status = item.Status
                };
                return eventModel;
            }));
            return detailItem;
        }));
        response.Total = comboParlayDetailViewModel.TotalWinLost;
        response.Outstanding = comboParlayDetailViewModel.OutStanding;
        return response;
    }

    private string GetWinLoss(string winLost, string comm)
    {
        return $"<div class='bl_underdog divWidth80'>{winLost}</div><div class='bl_underdog divWidth80'>{comm}</div>";
    }

    private string GetParlayBoost(string originalOdds, string boostOdds, string boostRate)
    {
        return $"<span class='underdog strikethrough'>{originalOdds}</span><br/><span class='oddstype'><span class='underdog'>{boostOdds}</span><br/><span class='boostrate'><span class='icon-boost'></span>{boostRate}</span></span>";
    }

    private string GetEventsCombination(List<string> events)
    {
        return string.Join("\n", events.Select(eventName => $"<div class='divEvent'>{eventName}</div>"));
    }
}
