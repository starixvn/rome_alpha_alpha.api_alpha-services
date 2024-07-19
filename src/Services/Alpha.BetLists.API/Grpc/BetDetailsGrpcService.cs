using Alpha.BetLists.API.Builders;
using Alpha.BetLists.API.Builders.StatusBuilders;
using Alpha.BetLists.API.Builders.WinLossBuilders;
using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Params;
using Alpha.Utilities.Helpers;
using Fanex.BetList.Core.Entities;
using Grpc.Core;
using Alpha.Utilities.Extensions;
using System.Text.Json;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    private const int MaxPagingSize = 1000000;

    public override async Task<SuperViewTicketDetailsResponse> SuperViewTicketDetails(GetTicketDetailsRequest request, ServerCallContext context)
    {
        var response = new SuperViewTicketDetailsResponse();
        var param = CreateCasinoDetailsParam(request);
        if (!ticketDetailsMapper.CheckProductIdIsAllowed(param.ProductId) 
            || param.TransType < 0 || request.RefNo != param.RefNo)
        {
            return response;
        }

        var (ticketDetails, ticketData, countries) = await GetTicketDetails(param);

        var i = 1;

        response.TicketDetails.AddRange(ticketDetails.Tickets.Select((t) => new SuperViewTicketDetailsModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t).CorrectDateTimeSpace(),
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

        var param = CreateCasinoDetailsParam(request);
        if (!ticketDetailsMapper.CheckProductIdIsAllowed(param.ProductId) 
            || param.TransType < 0
            || request.RefNo != param.RefNo)
        {
            return response;
        }

        var (ticketDetails, ticketData, countries) = await GetTicketDetails(param);

        var i = 1;
        response.TicketDetails.AddRange(ticketDetails.Tickets.Select((t) => new MasterViewTicketDetailsModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t).CorrectDateTimeSpace(),
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

        var param = CreateCasinoDetailsParam(request);
        if (!ticketDetailsMapper.CheckProductIdIsAllowed(param.ProductId) 
            || param.TransType < 0 
            || request.RefNo != param.RefNo)
        {
            return response;
        }

        var (ticketDetails, ticketData, countries) = await GetTicketDetails(param);
        var i = 1;
        response.TicketDetails.AddRange(ticketDetails.Tickets.Select((t) => new AgentViewTicketDetailsModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t).CorrectDateTimeSpace(),
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

    private async Task<(PagedData<TicketDto>, List<ITicketData>, IDictionary<string, string>)> GetTicketDetails(CasinoDetailsParam param)
    {
        var ticketDetails = casinoDetailsService.GetCasinoDetails(param);

        var ipList = ticketDetails.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        List<ITicketData> ticketData = new();
        ticketData.GetTicketData(ticketDetails.Tickets);

        return (ticketDetails, ticketData, countries);
    }

    private CasinoDetailsParam CreateCasinoDetailsParam(GetTicketDetailsRequest request)
    {
        var productId = request.ProductId;
        var transType = request.TransType;
        if (!string.IsNullOrEmpty(request.DetailKey))
        {
            var key = AesService.Decrypt(request.DetailKey);
            dynamic get3rdProductTicketFromKey = JsonSerializer.Deserialize<object>(key);
            if (get3rdProductTicketFromKey != null)
            {
                productId = get3rdProductTicketFromKey.ProductId;
                transType = get3rdProductTicketFromKey.TransType;
                
                return new CasinoDetailsParam(
                    get3rdProductTicketFromKey.RefNo, request.CustId, (byte)transType, productId, 1, MaxPagingSize);
            }
        }

        return new CasinoDetailsParam(
            request.RefNo, request.CustId, (byte)transType, productId, 1, MaxPagingSize);
    }
}