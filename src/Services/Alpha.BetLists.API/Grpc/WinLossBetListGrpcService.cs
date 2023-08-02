
using Grpc.Core;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Builders;
using Alpha.BetLists.API.Builders.WinLossBuilders;
using Alpha.BetLists.API.Builders.StatusBuilders;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    public override async Task<SuperViewWinLossBetListResponse> SuperViewWinLossBetList(GetWinLossBetListRequest request, ServerCallContext context)
    {
        var productIds = productService.GetAllProductIds(request.ProductGroup);

        var param = new GetWinLossBetListParam(
            request.CustId,
            request.RoleId,
            request.FromDate,
            request.ToDate,
            request.Language,
            request.CustName,
            request.Site,
            productIds,
            request.ViewerId,
            settings);

        var betlistData = await winLossBetListService.GetBetList(param);

        var response = new SuperViewWinLossBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;

        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        var htmlData = betlistData.Tickets.Select(t => new SuperViewWinLossBetTicketModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            WinLoss = WinLossBuilder.BuildWinLoss(t),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            SuperPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Super),
            MasterPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Master),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
        });

        response.BetLists.AddRange(htmlData);

        var total = WinLossTotalModel.GetWinLossTotal(betlistData);
        response.Total = new WinLossTotalModelResponse
        {
            Comm = total.Comm,
            WinLoss = total.WinLoss,
            Total = total.Total
        };

        return response;
    }

    public override async Task<MasterViewWinLossBetListResponse> MasterViewWinLossBetList(GetWinLossBetListRequest request, ServerCallContext context)
    {
        var productIds = productService.GetAllProductIds(request.ProductGroup);

        var param = new GetWinLossBetListParam(
            request.CustId,
            request.RoleId,
            request.FromDate,
            request.ToDate,
            request.Language,
            request.CustName,
            request.Site,
            productIds,
            request.ViewerId,
            settings);

        var betlistData = await winLossBetListService.GetBetList(param);

        var response = new MasterViewWinLossBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;

        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        response.BetLists.AddRange(betlistData.Tickets.Select(t => new MasterViewWinLossBetTicketModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            WinLoss = WinLossBuilder.BuildWinLoss(t),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            MasterPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Master),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
        }));

        var total = WinLossTotalModel.GetWinLossTotal(betlistData);
        response.Total = new WinLossTotalModelResponse
        {
            Comm = total.Comm,
            WinLoss = total.WinLoss,
            Total = total.Total
        };

        return response;
    }

    public override async Task<AgentViewWinLossBetListResponse> AgentViewWinLossBetList(GetWinLossBetListRequest request, ServerCallContext context)
    {
        var productIds = productService.GetAllProductIds(request.ProductGroup);

        var param = new GetWinLossBetListParam(
            request.CustId,
            request.RoleId,
            request.FromDate,
            request.ToDate,
            request.Language,
            request.CustName,
            request.Site,
            productIds,
            request.ViewerId,
            settings);

        var betlistData = await winLossBetListService.GetBetList(param);

        var response = new AgentViewWinLossBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;

        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        response.BetLists.AddRange(betlistData.Tickets.Select(t => new AgentViewWinLossBetTicketModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            WinLoss = WinLossBuilder.BuildWinLoss(t),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
        }));

        var total = WinLossTotalModel.GetWinLossTotal(betlistData);
        response.Total = new WinLossTotalModelResponse
        {
            Comm = total.Comm,
            WinLoss = total.WinLoss,
            Total = total.Total
        };

        return response;
    }
}