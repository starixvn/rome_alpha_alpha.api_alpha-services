using Alpha.BetLists.API.Builders;
using Alpha.BetLists.API.Builders.StatusBuilders;
using Alpha.BetLists.API.Builders.WinLossBuilders;
using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Params;
using Fanex.BetList.Core.Entities;
using Grpc.Core;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    public override async Task<SuperViewTicketDetailsResponse> SuperViewTicketDetails(GetTicketDetailsRequest request, ServerCallContext context)
    {
        var response = new SuperViewTicketDetailsResponse();
        if (!ticketDetailsMapper.CheckProductIdIsAllowed(request.ProductId) || request.TransType < 0)
        {
            return response;
        }
        var param = CreateCasinoDetailsParam(request);
        var ticketDetails = casinoDetailsService.GetCasinoDetails(param);

        var ipList = ticketDetails.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        List<ITicketData> ticketData = new();
        ticketData.GetTicketData(ticketDetails.Tickets);

        var i = 1;
        response.TicketDetails.AddRange(ticketDetails.Tickets.Select((t) => new SuperViewTicketDetailsModel
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
            Platform = t.Platform
        }
        ));
        return response;
    }

    public override async Task<MasterViewTicketDetailsResponse> MasterViewTicketDetails(GetTicketDetailsRequest request, ServerCallContext context)
    {
        var response = new MasterViewTicketDetailsResponse();

        if (!ticketDetailsMapper.CheckProductIdIsAllowed(request.ProductId) || request.TransType < 0)
        {
            return response;
        }
        var param = CreateCasinoDetailsParam(request);

        var ticketDetails = casinoDetailsService.GetCasinoDetails(param);

        var ipList = ticketDetails.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        List<ITicketData> ticketData = new();
        ticketData.GetTicketData(ticketDetails.Tickets);

        var i = 1;
        response.TicketDetails.AddRange(ticketDetails.Tickets.Select((t) => new MasterViewTicketDetailsModel
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
            Platform = t.Platform
        }));
        return response;
    }

    public override async Task<AgentViewTicketDetailsResponse> AgentViewTicketDetails(GetTicketDetailsRequest request, ServerCallContext context)
    {
        var response = new AgentViewTicketDetailsResponse();

        if (!ticketDetailsMapper.CheckProductIdIsAllowed(request.ProductId) || request.TransType < 0)
        {
            return response;
        }
        var param = CreateCasinoDetailsParam(request);

        var ticketDetails = casinoDetailsService.GetCasinoDetails(param);

        var ipList = ticketDetails.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        List<ITicketData> ticketData = new();
        ticketData.GetTicketData(ticketDetails.Tickets);

        var i = 1;
        response.TicketDetails.AddRange(ticketDetails.Tickets.Select((t) => new AgentViewTicketDetailsModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            WinLoss = WinLossBuilder.BuildWinLoss(t),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
            Platform = t.Platform
        }));
        return response;
    }

    private CasinoDetailsParam CreateCasinoDetailsParam(GetTicketDetailsRequest request)
        => new CasinoDetailsParam(
            request.RefNo,
            request.CustId,
            (byte)request.TransType,
            request.ProductId, 1, 500);

}