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
using Alpha.Utilities.Helpers;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.Utilities.Models;
using System.Text.Json;
using BetListNPOI.HSSF.Record.Chart;
using Alpha.BetLists.API.Constants;

namespace Alpha.BetLists.API.Helpers
{
    public static class TicketDetailsHelper
    {
        public const byte SingleParlay = 1;
        public const byte MixParlay = 2;
        public const byte MixParlayDetail = 3;
        private static readonly byte[] ParlayProducts =
        {
            SingleParlay,
            MixParlay,
            MixParlayDetail,
        };

        private static readonly IDictionary<string, int[]> TicketDetailsSettingKeyMappers = new Dictionary<string, int[]>(){
            { "Acc_BetList_LC_Detail",
            new int[] { ProductIdConstants.Allbet, ProductIdConstants.SAGaming, ProductIdConstants.AESexy, ProductIdConstants.IBCBetLiveCasino,
            ProductIdConstants.ION, ProductIdConstants.PGSoft, ProductIdConstants.WMLiveCasino, ProductIdConstants.BGLiveCasino,
            ProductIdConstants.PPLiveCasino, ProductIdConstants.FGG, ProductIdConstants.YeeBet, ProductIdConstants.WELiveCasino,
            ProductIdConstants.ONCasino, ProductIdConstants.GPILiveCasino, ProductIdConstants.MicrogamingLive, ProductIdConstants.LiveCasinoDummy } },
            { "Acc_BetList_RNG_Detail",
                new int[] { ProductIdConstants.SabaCasino, ProductIdConstants.FunkyGamesOld, ProductIdConstants.Joker,
                ProductIdConstants.MaxGame,ProductIdConstants.Habanero, ProductIdConstants.CreativeGaming, ProductIdConstants.AdvantPlay,
                ProductIdConstants.AdvantPlayMini, ProductIdConstants.SEALEntertainment, ProductIdConstants.MGRNG, ProductIdConstants.Jili,
                ProductIdConstants.Live22, ProductIdConstants.UUSLOTS, ProductIdConstants.SABAxD, ProductIdConstants.FastSpin,
                ProductIdConstants.FACHAI, ProductIdConstants.Nextspin, ProductIdConstants.Playtech, ProductIdConstants.GPIRNG, ProductIdConstants.PLAYSTAR,
                ProductIdConstants.ASKMESLOT, ProductIdConstants.SabaKeno, ProductIdConstants.FUNKYGAMES, ProductIdConstants.HOTDOG, ProductIdConstants.OdinGaming,
                ProductIdConstants.RNGCasinoDummy } }
        };

        private static int GetProductIdByChoice(string choice, TicketDetailsMapperModel mapper)
        {
            var lowerChoice = choice.ToLower();

            foreach (var entry in mapper.Values)
            {
                if (lowerChoice.Contains(entry.Key.ToLower()))
                {
                    return entry.Value;
                }
            }

            return -1;
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

            var key = JsonSerializer.Serialize(new Get3rdProductTicketDetailModel(
                int.Parse(t.GetTransactionType()),
                productId,
                t.RefNo));
            return AesService.Encrypt(key);
        }

        public static void GetExtraInfo(this object model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
        {
            var productId = GetProductIdByChoice(choice, mapper);
            if (productId == -1)
            {
                return;
            }

            switch (model)
            {
                case SuperViewRunningBetTicketModel ticket:
                    ticket.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case MasterViewRunningBetTicketModel ticket:
                    ticket.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case AgentViewRunningBetTicketModel ticket:
                    ticket.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case SuperViewWinLossBetTicketModel ticket:
                    ticket.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case MasterViewWinLossBetTicketModel ticket:
                    ticket.ExtraInfo = new ExtraInfoModel
                    {
                        RefNo = t.TransId.ToString(),
                        TransType = int.Parse(t.GetTransactionType()),
                        ProductId = productId
                    };
                    break;
                case AgentViewWinLossBetTicketModel ticket:
                    break;
                default: break;
            }
        }

        public static byte GetParlayProductId(string choice, TicketDetailsMapperModel mapper)
        {
            var lowerChoice = choice.ToLower();
            foreach (var entry in mapper.ParlayProducts)
            {
                if (lowerChoice.Contains(entry.Key.ToLower()))
                {
                    return entry.Value;
                }
            }

            return 0;
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
            var key = JsonSerializer.Serialize(new DetailKeyModel<TicketDto>()
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

        public static StringBuilder GetMixParlaySelections(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
        {
            var tickets = parlayService.GetParlayBetList(t.TransId, t.CustId).Result;
            var message = new ParlayMeta()
            {
                BetCheck = t.BetCheck,
                BetTeam = t.BetTeam,
                Handicap1 = t.Handicap1,
                Handicap2 = t.Handicap2
            };
            var parlayDetail = new ComboParlayViewModel(message, new BetListData(tickets, null), ticketHelper);

            StringBuilder sb = new StringBuilder();
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
            return sb;
        }

        public static StringBuilder GetMixParlayDetail(TicketDto t, IParlayService parlayService, ITicketHelper ticketHelper, IBetListHTMLBuilder builder)
        {
            var tickets = parlayService.GetParlayBetList(t.TransId, t.CustId).Result;
            var message = new ParlayMeta()
            {
                BetCheck = t.BetCheck,
                BetTeam = t.BetTeam,
                Handicap1 = t.Handicap1,
                Handicap2 = t.Handicap2
            };
            var parlayDetail = new ComboParlayViewModel(message, new BetListData(tickets, null), ticketHelper);

            StringBuilder sb = new StringBuilder();
            if (parlayDetail.HasValidData)
            {
                foreach (var unitStakeByOfferNames in parlayDetail.UnitStakeByOfferNames)
                {
                    sb.Append("<div class='offer-name-list'>");
                    sb.Append($"{unitStakeByOfferNames.Key} x <span class='unit-stake'>{unitStakeByOfferNames.Value}</span>");
                    sb.Append("</div>");
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
            }
            return sb;
        }

        public static string GetTicketDetailSettingKey(int productId)
            => TicketDetailsSettingKeyMappers.FirstOrDefault(mapper => mapper.Value.Contains(productId)).Key;

    }
}