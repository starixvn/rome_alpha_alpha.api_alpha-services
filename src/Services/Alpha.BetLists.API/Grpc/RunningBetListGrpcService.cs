
using Grpc.Core;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Builders;
using Alpha.BetLists.API.Builders.StatusBuilders;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    public override async Task<SuperViewRunningBetListResponse> SuperViewRunningBetList(GetRunningBetListRequest request, ServerCallContext context)
    {
        var productIds = productService.GetAllProductIds(request.ProductGroup);

        var param = new GetRunningBetListParam(
            request.CustId,
            request.Language,
            request.CustName,
            request.Site,
            productIds);

        var betlistData = await runningBetListService.GetBetList(param);

        var response = new SuperViewRunningBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;

        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);

        var htmlData = betlistData.Tickets.Select(t => new SuperViewRunningBetTicketModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            SuperPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Super),
            MasterPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Master),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
        });

        response.BetLists.AddRange(htmlData);

        return response;
    }

    public override async Task<MasterViewRunningBetListResponse> MasterViewRunningBetList(GetRunningBetListRequest request, ServerCallContext context)
    {
        var productIds = productService.GetAllProductIds(request.ProductGroup);

        var param = new GetRunningBetListParam(
            request.CustId,
            request.Language,
            request.CustName,
            request.Site,
            productIds);

        var betlistData = await runningBetListService.GetBetList(param);

        var response = new MasterViewRunningBetListResponse();

        var i = 1;
        var ticketData = betlistData.TicketData;
        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);


        response.BetLists.AddRange(betlistData.Tickets.Select(t => new MasterViewRunningBetTicketModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            MasterPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Master),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
        }));

        return response;
    }

    public override async Task<AgentViewRunningBetListResponse> AgentViewRunningBetList(GetRunningBetListRequest request, ServerCallContext context)
    {
        logger.Info("AgentViewRunningBetList: GetAllProductIds");
        var productIds = productService.GetAllProductIds(request.ProductGroup);

        logger.Info("AgentViewRunningBetList: GetBetList");
        var param = new GetRunningBetListParam(
            request.CustId,
            request.Language,
            request.CustName,
            request.Site,
            productIds);

        var betlistData = await runningBetListService.GetBetList(param);

        var response = new AgentViewRunningBetListResponse();

        logger.Info("AgentViewRunningBetList: GetCountries");

        var i = 1;
        var ticketData = betlistData.TicketData;
        var ipList = betlistData.Tickets.Where(t => !string.IsNullOrWhiteSpace(t.IP)).Select(t => t.IP.Trim()).Distinct();
        var countries = await ipInfoService.GetCountries(ipList);


        response.BetLists.AddRange(betlistData.Tickets.Select(t => new AgentViewRunningBetTicketModel
        {
            No = i++.ToString(),
            TransTime = builder.BuildTrans(t),
            Choice = builder.BuildChoice(t, ticketHelper, ticketData, false),
            Odds = builder.BuildOdds(t, ticketData, ticketHelper.GetOddsTypeNameById),
            Stake = builder.BuildStake(t, ticketHelper),
            Status = builder.BuildStatus(t, ticketHelper, ticketData, GetCountry(t.IP, countries)),
            AgentPTComm = BetListHtmlBuilderExtensions.BuildPtComm(t, ticketData, UserLevel.Agent),
        }));

        logger.Info("AgentViewRunningBetList: BuildTrans");
        return response;
    }
}