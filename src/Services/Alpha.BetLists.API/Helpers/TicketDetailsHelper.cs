using Alpha.BetLists.API.Models;
using GrpcBetLists;
using Fanex.BetList.Core.Utils.TicketExt;
using Fanex.BetList.Core.Entities;
using System.Text;
using Fanex.BetList.Core.Builder;
using Alpha.BetLists.API.Services;
using Alpha.BetLists.API.Models.Parlay;
using Fanex.BetDetails.BetBuilder.Criteria;
using Fanex.BetDetails.BetBuilder.Services;
using Newtonsoft.Json;
using Alpha.Utilities.Helpers;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.Utilities.Models;
using Alpha.BetLists.API.Constants;
using Fanex.BetList.Core.Builder.HybridParlayBuilder;

namespace Alpha.BetLists.API.Helpers
{
    public static class TicketDetailsHelper
    {
        public const byte SingleParlay = 1;
        public const byte MixParlay = 2;
        public const byte BetBuilderPlus = 3;

        private static readonly IDictionary<string, int[]> TicketDetailsSettingKeyMappers = new Dictionary<string, int[]>
        {
            { "Acc_BetList_LC_Detail",
            new [] { ProductIdConstants.Allbet, ProductIdConstants.SAGaming, ProductIdConstants.AESexy, ProductIdConstants.IBCBetLiveCasino,
            ProductIdConstants.ION, ProductIdConstants.PGSoft, ProductIdConstants.WMLiveCasino, ProductIdConstants.BGLiveCasino,
            ProductIdConstants.PPLiveCasino, ProductIdConstants.FGG, ProductIdConstants.YeeBet, ProductIdConstants.WELiveCasino,
            ProductIdConstants.ONCasino, ProductIdConstants.GPILiveCasino, ProductIdConstants.MicrogamingLive, ProductIdConstants.LiveCasinoDummy }
            },
            { "Acc_BetList_RNG_Detail",
                new [] { ProductIdConstants.SabaCasino, ProductIdConstants.FunkyGamesOld, ProductIdConstants.Joker,
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
            var lowerChoice = choice.ToLowerInvariant();
            var entry = mapper.Values.FirstOrDefault(entry => lowerChoice.Contains(entry.Key.ToLowerInvariant()));
            
            return entry.Key != null ? entry.Value : -1;
        }

        public static bool CheckProductIdIsAllowed(this TicketDetailsMapperModel mapper, int productId)
        {
            foreach (var entry in mapper.Values)
            {
                if (productId == entry.Value)
                {
                    return true;
                }
            }

            return false;
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

        private static void SetExtraInfo(ExtraInfoModel extraInfo, object model)
        {
            switch (model)
            {
                case SuperViewRunningBetTicketModel ticket:
                    ticket.ExtraInfo = extraInfo;
                    break;
                case MasterViewRunningBetTicketModel ticket:
                    ticket.ExtraInfo = extraInfo;
                    break;
                case AgentViewRunningBetTicketModel ticket:
                    ticket.ExtraInfo = extraInfo;
                    break;
                case SuperViewWinLossBetTicketModel ticket:
                    ticket.ExtraInfo = extraInfo;
                    break;
                case MasterViewWinLossBetTicketModel ticket:
                    ticket.ExtraInfo = extraInfo;
                    break;
                default:
                    return;
            }
        }

        private static void GetExtraInfoInternal(object model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
        {
            var productId = GetProductIdByChoice(choice, mapper);
            if (productId == -1)
            {
                return;
            }

            var extraInfo = new ExtraInfoModel
            {
                RefNo = t.TransId.ToString(),
                TransType = int.Parse(t.GetTransactionType()),
                ProductId = productId
            };

            SetExtraInfo(extraInfo, model);
        }

        public static void GetExtraInfo(this SuperViewRunningBetTicketModel model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            => GetExtraInfoInternal(model, choice, t, mapper);

        public static void GetExtraInfo(this MasterViewRunningBetTicketModel model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            => GetExtraInfoInternal(model, choice, t, mapper);

        public static void GetExtraInfo(this AgentViewRunningBetTicketModel model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            => GetExtraInfoInternal(model, choice, t, mapper);

        public static void GetExtraInfo(this SuperViewWinLossBetTicketModel model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            => GetExtraInfoInternal(model, choice, t, mapper);

        public static void GetExtraInfo(this MasterViewWinLossBetTicketModel model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            => GetExtraInfoInternal(model, choice, t, mapper);

        public static void GetExtraInfo(this AgentViewWinLossBetTicketModel model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
            => GetExtraInfoInternal(model, choice, t, mapper);

        public static byte GetParlayProductId(string choice, TicketDetailsMapperModel mapper)
        {
            var lowerChoice = choice.ToLowerInvariant();
            var productId = mapper.ParlayProducts.FirstOrDefault(entry => lowerChoice.Contains(entry.Key.ToLowerInvariant())).Value;
            
            return productId;
        }

        public static void GetTicketData(this List<ITicketData> list, IEnumerable<ITicket> tickets)
        {
            list.AddRange(tickets.Select((detail) => new TicketData
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

        public static StringBuilder GetSingleParlaySelections(TicketDto t, IBetBuilderService betBuilderService)
        {
            var parameter = new GetBetBuilderDetailParameter
            {
                CustomerId = t.CustId,
                TransactionId = t.TransId,
                Status = t.Status
            };
            var parlayDetail = betBuilderService.GetBetListDetail(parameter);
            StringBuilder sb = new();
            if (parlayDetail.Details.Any())
            {
                sb.Append("<div class='betbuilder'>");
                foreach (var detail in parlayDetail.Details)
                {
                    sb.Append($"<div>{detail.SelectionName}");
                    if (parlayDetail.IsSettled && !string.IsNullOrEmpty(detail.Result))
                    {
                        sb.Append($"<span> - {detail.Result}</span>");
                    }
                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
            return sb;
        }

        public static async Task<string> GetMixParlaySelections(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
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

            StringBuilder sb = new();
            if (parlayDetail.HasValidData)
            {
                foreach (var unitStakeByOfferNames in parlayDetail.UnitStakeByOfferNames)
                {
                    sb.Append("<div class='offer-name-list'>");
                    sb.Append($"{unitStakeByOfferNames.Key} x <span class='unit-stake'>{unitStakeByOfferNames.Value}</span>");
                    sb.Append("</div>");
                }
                sb.Append("<div class='line'></div>");
                foreach (var ticket in parlayDetail.BetListData.Tickets)
                {
                    sb.Append("<div class='ticket-list'>");
                    sb.Append($"{builder.BuildChoice(ticket, ticketHelper, null, false).Replace("\"", "'")}");
                    sb.Append($"{builder.BuildTransEventDateAndStatusResult(ticket, null).Replace("\"", "'")}");
                    sb.Append("</div>");
                    if (!ticket.Equals(parlayDetail.BetListData.Tickets.Last()))
                    {
                        sb.Append("<div class='line'></div>");
                    }
                }
            }
            return sb.ToString();
        }

        public static async Task<string> GetBetBuilderPlusSelections(TicketDto t, IParlayService parlayService,ITicketHelper ticketHelper, IHybridParlayBuilder hybridParlayBuilder)
        {
            var tickets = await parlayService.GetParlayBetList(t.TransId, t.CustId);
            if(!tickets.Any())
            {
                return string.Empty;
            }

            var ticketHasStake = tickets.FirstOrDefault(t => t.Stake > 0) ?? tickets.First();
            var hybridParlay = hybridParlayBuilder.Build(
                ticketHelper, 
                tickets.Select(t => t as ITicket).ToList(), 
                t.TransDesc, 
                (int)t.Handicap1, 
                (int)t.Handicap2, 
                t.BetCheck, 
                t.BetTeam, 
                ticketHasStake.Currency, ticketHasStake.Stake,
                false);
            
            return hybridParlay.Replace("\"", "'");
        }

        public static async Task<StringBuilder> GetMixParlayDetail(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
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

            StringBuilder sb = new();
            if (parlayDetail.HasValidData)
            {
                foreach (var unitStakeByOfferNames in parlayDetail.UnitStakeByOfferNames)
                {
                    sb.Append("<div class='offer-name-list'>");
                    sb.Append($"{unitStakeByOfferNames.Key} x <span class='unit-stake'>{unitStakeByOfferNames.Value}</span>");
                    sb.Append("</div>");
                    sb.Append("<div class='line'></div>");
                    var selectionList = new List<string>();
                    foreach (var ticket in parlayDetail.BetListData.Tickets)
                    {
                        var selection = new StringBuilder();
                        selection.Append("<div class='ticket-list'>");
                        selection.Append($"{builder.BuildChoice(ticket, ticketHelper, null, false).Replace("\"", "'")}");
                        selection.Append($"{builder.BuildTransEventDateAndStatusResult(ticket, null).Replace("\"", "'")}");
                        selection.Append("</div>");
                        selectionList.Add(selection.ToString());
                    }
                    sb.Append(string.Join("<div class='line'></div>", selectionList));
                }
            }
            return sb;
        }

        public static string GetTicketDetailSettingKey(int productId)
            => TicketDetailsSettingKeyMappers.FirstOrDefault(mapper => mapper.Value.Contains(productId)).Key;

    }
}