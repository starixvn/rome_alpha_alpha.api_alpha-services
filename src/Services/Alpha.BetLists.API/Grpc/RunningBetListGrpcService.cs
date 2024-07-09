
using Grpc.Core;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Builders;
using Alpha.BetLists.API.Builders.StatusBuilders;
using Alpha.BetLists.API.Helpers;
using Alpha.Utilities.Extensions;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    public override async Task<SuperViewRunningBetListResponse> SuperViewRunningBetList(GetRunningBetListRequest request, ServerCallContext context)
    {
        var productIds = string.IsNullOrWhiteSpace(request.ProductIds)
            ? productService.GetAllProductIds(request.ProductGroup)
            : request.ProductIds.ToListInt();

        var param = new GetRunningBetListParam(
            request.CustId,
            request.CustName,
            request.Site,
            productIds);

        var betlistData = await runningBetListService.GetBetList(param);

        var response = new SuperViewRunningBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;

        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        response.BetLists.AddRange(betlistData.Tickets.Select((t) =>
        {
            var choice = builder.BuildChoice(t, ticketHelper, ticketData, false);
            var model = new SuperViewRunningBetTicketModel
            {
                No = i++.ToString(),
                TransTime = builder.BuildTrans(t).CorrectDateTimeSpace(),
                Choice = choice,
                Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
                Stake = builder.BuildStake(t, ticketHelper),
                Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
                SuperPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Super),
                MasterPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Master),
                AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
                Platform = t.Platform
            };

            model.GetExtraInfo(choice, t, ticketDetailsMapper);
            model.DetailKey = TicketDetailsHelper.GetDetailKey(choice, t, ticketDetailsMapper);

            return model;
        }));

        return response;
    }

    public override async Task<MasterViewRunningBetListResponse> MasterViewRunningBetList(GetRunningBetListRequest request, ServerCallContext context)
    {
        var productIds = string.IsNullOrWhiteSpace(request.ProductIds)
            ? productService.GetAllProductIds(request.ProductGroup)
            : request.ProductIds.ToListInt();

        var param = new GetRunningBetListParam(
            request.CustId,
            request.CustName,
            request.Site,
            productIds);

        var betlistData = await runningBetListService.GetBetList(param);

        var response = new MasterViewRunningBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;
        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);


        response.BetLists.AddRange(betlistData.Tickets.Select(t =>
        {
            var choice = builder.BuildChoice(t, ticketHelper, ticketData, false);

            var model = new MasterViewRunningBetTicketModel
            {
                No = i++.ToString(),
                TransTime = builder.BuildTrans(t).CorrectDateTimeSpace(),
                Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
                Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
                Stake = builder.BuildStake(t, ticketHelper),
                Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
                MasterPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Master),
                AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
                Platform = t.Platform
            };

            model.GetExtraInfo(choice, t, ticketDetailsMapper);
            model.DetailKey = TicketDetailsHelper.GetDetailKey(choice, t, ticketDetailsMapper);

            return model;
        }));

        return response;
    }

    public override async Task<AgentViewRunningBetListResponse> AgentViewRunningBetList(GetRunningBetListRequest request, ServerCallContext context)
    {
        var productIds = string.IsNullOrWhiteSpace(request.ProductIds)
            ? productService.GetAllProductIds(request.ProductGroup)
            : request.ProductIds.ToListInt();

        var param = new GetRunningBetListParam(
            request.CustId,
            request.CustName,
            request.Site,
            productIds);

        var betlistData = await runningBetListService.GetBetList(param);

        var response = new AgentViewRunningBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;
        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        response.BetLists.AddRange(betlistData.Tickets.Select(t =>
        {
            var choice = builder.BuildChoice(t, ticketHelper, ticketData, false);
            var model = new AgentViewRunningBetTicketModel
            {
                No = i++.ToString(),
                TransTime = builder.BuildTrans(t).CorrectDateTimeSpace(),
                Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
                Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
                Stake = builder.BuildStake(t, ticketHelper),
                Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
                AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
                Platform = t.Platform
            };
            model.GetExtraInfo(choice, t, ticketDetailsMapper);
            model.DetailKey = TicketDetailsHelper.GetDetailKey(choice, t, ticketDetailsMapper);

            return model;
        }));

        return response;
    }
}