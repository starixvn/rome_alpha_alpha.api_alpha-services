using Alpha.BetLists.API.Constants;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.Parlay;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.BetLists.API.Services;
using Alpha.Utilities.Helpers;
using Alpha.Utilities.Models;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Utils.TicketExt;
using GrpcBetLists;
using Newtonsoft.Json;
using System.Text;

namespace Alpha.BetLists.API.Helpers
{
    public static class TicketDetailsHelper
    {
        private const string DivCloseTag = "</div>";
        private const string DivLineTag = "<div class='line'></div>"; // Added constant
        public const byte SingleParlay = 1;
        public const byte MixParlay = 2;
        public const byte MixParlayDetail = 3;

        private static readonly IDictionary<string, int[]> TicketDetailsSettingKeyMappers = new Dictionary<string, int[]>{
            { "Acc_BetList_LC_Detail",
                new[] { ProductIdConstants.Allbet, ProductIdConstants.SAGaming, ProductIdConstants.AESexy, ProductIdConstants.IBCBetLiveCasino,
                ProductIdConstants.ION, ProductIdConstants.PGSoft, ProductIdConstants.WMLiveCasino, ProductIdConstants.BGLiveCasino,
                ProductIdConstants.PPLiveCasino, ProductIdConstants.FGG, ProductIdConstants.YeeBet, ProductIdConstants.WELiveCasino,
                ProductIdConstants.ONCasino, ProductIdConstants.GPILiveCasino, ProductIdConstants.MicrogamingLive, ProductIdConstants.LiveCasinoDummy }
            },
            { "Acc_BetList_RNG_Detail",
                new[] { ProductIdConstants.SabaCasino, ProductIdConstants.FunkyGamesOld, ProductIdConstants.Joker,
                ProductIdConstants.MaxGame,ProductIdConstants.Habanero, ProductIdConstants.CreativeGaming, ProductIdConstants.AdvantPlay,
                ProductIdConstants.AdvantPlayMini, ProductIdConstants.SEALEntertainment, ProductIdConstants.MGRNG, ProductIdConstants.Jili,
                ProductIdConstants.Live22, ProductIdConstants.UUSLOTS, ProductIdConstants.SABAxD, ProductIdConstants.FastSpin,
                ProductIdConstants.FACHAI, ProductIdConstants.Nextspin, ProductIdConstants.Playtech, ProductIdConstants.GPIRNG, ProductIdConstants.PLAYSTAR,
                ProductIdConstants.ASKMESLOT, ProductIdConstants.SabaKeno, ProductIdConstants.FUNKYGAMES, ProductIdConstants.HOTDOG, ProductIdConstants.OdinGaming,
                ProductIdConstants.RNGCasinoDummy }
            }
        };

        private static int GetProductIdByChoice(string choice, TicketDetailsMapperModel mapper)
        {
            var lowerChoice = choice.ToLower(System.Globalization.CultureInfo.InvariantCulture);

            var entry = mapper.Values
                .FirstOrDefault(e => lowerChoice.Contains(e.Key, StringComparison.CurrentCultureIgnoreCase));

            return entry.Equals(default(KeyValuePair<string, int>)) ? -1 : entry.Value;
        }

        public static bool CheckProductIdIsAllowed(this TicketDetailsMapperModel mapper, int productId)
        {
            return mapper.Values.Any(entry => productId == entry.Value);
        }

        public static string GetDetailKey(string choice, TicketDto t, TicketDetailsMapperModel mapper)
        {
            var parlayProductId = GetParlayProductId(choice, mapper);
            if (parlayProductId == 0)
            {
                return Get3rdProductDetailKey(choice, t, mapper);
            }

            return GetParlayDetailKey(t, parlayProductId, choice);
        }

        private static string Get3rdProductDetailKey(string choice, TicketDto t, TicketDetailsMapperModel mapper)
        {
            var productId = GetProductIdByChoice(choice, mapper);
            if (productId == -1)
            {
                return string.Empty;
            }

            var key = JsonConvert.SerializeObject(new Get3rdProductTicketDetailModel(
                int.Parse(t.GetTransactionType()),
                productId));
            return AesService.Encrypt(key);
        }

        public static void GetExtraInfo<T>(this T model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            where T : class
        {
            var productId = GetProductIdByChoice(choice, mapper);
            if (productId == -1)
            {
                return;
            }

            switch (model)
            {
                case SuperViewRunningBetTicketModel superViewRunningBetTicketModel:
                    superViewRunningBetTicketModel.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case MasterViewRunningBetTicketModel masterViewRunningBetTicketModel:
                    masterViewRunningBetTicketModel.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case AgentViewRunningBetTicketModel agentViewRunningBetTicketModel:
                    agentViewRunningBetTicketModel.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case SuperViewWinLossBetTicketModel superViewWinLossBetTicketModel:
                    superViewWinLossBetTicketModel.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case MasterViewWinLossBetTicketModel masterViewWinLossBetTicketModel:
                    masterViewWinLossBetTicketModel.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case AgentViewWinLossBetTicketModel:
                    // No assignment needed, so do nothing
                    break;
                default:
                    // No action needed for other types
                    break;
            }
        }

        // Remove redundant parentheses in the following lines:

        public static byte GetParlayProductId(string choice, TicketDetailsMapperModel mapper)
        {
            var lowerChoice = choice.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            var entry = mapper.ParlayProducts
                .FirstOrDefault(e => lowerChoice.Contains(e.Key, StringComparison.CurrentCultureIgnoreCase));

            return entry.Equals(default(KeyValuePair<string, byte>)) ? (byte)0 : entry.Value;
        }

        public static void GetTicketData(this List<ITicketData> list, IEnumerable<ITicket> tickets)
        {
            list.AddRange(tickets.Select(detail => new TicketData
            {
                RefNo = detail.TransId.ToString(),
                BetTypeId = detail.BetTypeId,
                Status = detail.Status,
                Odds = detail.Odds,
                Stake = detail.Stake,
                BetTeam = detail.BetTeam,
                TransDesc = detail.TransDesc,
                SuperPT = detail.SuperPositionTaking,
                MasterPT = detail.MasterPositionTaking,
                AgentPT = detail.AgentPositionTaking
            }));
        }

        private static string GetParlayDetailKey(TicketDto t, byte parlayProductId, string choice)
        {
            var key = JsonConvert.SerializeObject(new DetailKeyModel<TicketDto>
            {
                ticket = t,
                ParlayProductId = parlayProductId,
                RefNo = t.TransId,
                SupportDetails = choice.Contains("detail-link")
            });

            return AesService.Encrypt(key);
        }

        public static async Task<StringBuilder> GetSingleParlaySelections(TicketDto t, IBetBuilderService betBuilderService)
        {
            var settledStatus = new List<string> { BetStatus.Won, BetStatus.Lose, BetStatus.Refund };
            var details = await betBuilderService.GetBetList(t.CustId, t.TransId);
            StringBuilder sb = new();
            if (details.Any())
            {
                sb.Append("<div class='betbuilder'>");
                foreach (var detail in details)
                {
                    sb.Append($"<div>{detail.SelectionName}");
                    if (settledStatus.Exists(i => i.Equals(t.Status)) && !string.IsNullOrEmpty(detail.Result))
                    {
                        sb.Append($"<span> - {detail.Result}</span>");
                    }
                    sb.Append(DivCloseTag);
                }
                sb.Append(DivCloseTag);
            }
            return sb;
        }

        private static void AppendUnitStakeByOfferNames(StringBuilder sb, ComboParlayViewModel parlayDetail)
        {
            foreach (var unitStakeByOfferNames in parlayDetail.UnitStakeByOfferNames)
            {
                sb.Append("<div class='offer-name-list'>");
                sb.Append($"{unitStakeByOfferNames.Key} x <span class='unit-stake'>{unitStakeByOfferNames.Value}</span>");
                sb.Append(DivCloseTag);
            }
        }

        private static void AppendTickets(StringBuilder sb, ComboParlayViewModel parlayDetail, IBetListHTMLBuilder builder, ITicketHelper ticketHelper)
        {
            var tickets = parlayDetail.BetListData.Tickets.ToList();
            for (int i = 0; i < tickets.Count; i++)
            {
                sb.Append("<div class='ticket-list'>");
                sb.Append($"{builder.BuildChoice(tickets[i], ticketHelper, null, false).Replace("\"", "'")}");
                sb.Append($"{builder.BuildTransEventDateAndStatusResult(tickets[i], null).Replace("\"", "'")}");
                sb.Append(DivCloseTag);
                if (i != tickets.Count - 1)
                {
                    sb.Append(DivLineTag); // Use constant
                }
            }
        }

        public static async Task<StringBuilder> GetMixParlaySelectionsAsync(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
        {
            var tickets = await parlayService.GetParlayBetList(t.TransId, t.CustId);
            var message = new ParlayMeta
            {
                BetCheck = t.BetCheck,
                BetTeam = t.BetTeam,
                Handicap1 = t.Handicap1,
                Handicap2 = t.Handicap2
            };
            var parlayDetail = new ComboParlayViewModel(message, new BetListData(tickets, null), ticketHelper);

            var sb = new StringBuilder();
            if (!parlayDetail.HasValidData)
            {
                return sb;
            }

            AppendUnitStakeByOfferNames(sb, parlayDetail);
            sb.Append(DivLineTag); // Use constant
            AppendTickets(sb, parlayDetail, builder, ticketHelper);

            return sb;
        }

        public static async Task<StringBuilder> GetMixParlayDetailAsync(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
        {
            var tickets = await parlayService.GetParlayBetList(t.TransId, t.CustId);
            var message = new ParlayMeta
            {
                BetCheck = t.BetCheck,
                BetTeam = t.BetTeam,
                Handicap1 = t.Handicap1,
                Handicap2 = t.Handicap2
            };
            var parlayDetail = new ComboParlayViewModel(message, new BetListData(tickets, null), ticketHelper);

            var sb = new StringBuilder();
            if (!parlayDetail.HasValidData)
            {
                return sb;
            }

            foreach (var unitStakeByOfferNames in parlayDetail.UnitStakeByOfferNames)
            {
                sb.Append("<div class='offer-name-list'>");
                sb.Append($"{unitStakeByOfferNames.Key} x <span class='unit-stake'>{unitStakeByOfferNames.Value}</span>");
                sb.Append(DivCloseTag);
                sb.Append(DivLineTag); // Use constant

                var ticketList = parlayDetail.BetListData.Tickets.ToList();
                int lastIndex = ticketList.Count - 1;
                for (int i = 0; i < ticketList.Count; i++)
                {
                    var ticket = ticketList[i];
                    sb.Append("<div class='ticket-list'>");
                    sb.Append($"{builder.BuildChoice(ticket, ticketHelper, null, false).Replace("\"", "'")}");
                    sb.Append($"{builder.BuildTransEventDateAndStatusResult(ticket, null).Replace("\"", "'")}");
                    sb.Append(DivCloseTag);
                    if (i != lastIndex)
                    {
                        sb.Append(DivLineTag); // Use constant
                    }
                }
            }

            return sb;
        }

        // Make GetMixParlayDetail call GetMixParlayDetailAsync to avoid duplicate implementation
        public static Task<StringBuilder> GetMixParlayDetail(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
        {
            return GetMixParlayDetailAsync(t, parlayService, ticketHelper, builder);
        }

        public static string GetTicketDetailSettingKey(int productId)
            => TicketDetailsSettingKeyMappers.FirstOrDefault(mapper => mapper.Value.Contains(productId)).Key;
    }
}