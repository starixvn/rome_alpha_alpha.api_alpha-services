using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Services;
using Alpha.IPInfo.Services;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Microsoft.Extensions.Options;

namespace GrpcBetLists;

public partial class BetListsGrpcService : BetLists.BetListsBase
{
    private readonly BetListsAppSettingsModel settings;
    private readonly IProductService productService;
    private readonly IBetListHTMLBuilder builder;
    private readonly ITicketHelper ticketHelper;
    private readonly IRunningBetListService runningBetListService;
    private readonly IWinLossBetListService winLossBetListService;
    private readonly IIPInfoService ipInfoService;
    private readonly Fanex.Logging.ILogger logger;

    public BetListsGrpcService(
        IOptions<BetListsAppSettingsModel> settings,
        IProductService productService,
        IBetListHTMLBuilder builder,
        ITicketHelper ticketHelper,
        IRunningBetListService runningBetListService,
        IWinLossBetListService winLossBetListService,
        IIPInfoService ipInfoService,
        Fanex.Logging.ILogger logger)
    {
        this.settings = settings.Value;
        this.productService = productService;
        this.builder = builder;
        this.ticketHelper = ticketHelper;
        this.runningBetListService = runningBetListService;
        this.winLossBetListService = winLossBetListService;
        this.ipInfoService = ipInfoService;
        this.logger = logger;
    }

    private static string GetCountry(string ip, IDictionary<string, string> countries)
        => !string.IsNullOrWhiteSpace(ip) && countries.TryGetValue(ip.Trim(), out string r) ? r : "-";
}