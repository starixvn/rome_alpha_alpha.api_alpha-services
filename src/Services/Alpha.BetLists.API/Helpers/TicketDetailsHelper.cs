using Alpha.BetLists.API.Models;
using GrpcBetLists;
using Fanex.BetList.Core.Utils.TicketExt;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Helpers
{
    public static class TicketDetailsHelper
    {
        private static int GetProductIdByChoice(string choice, TicketDetailsMapperModel mapper)
        {
            foreach (var entry in mapper.Values)
            {
                if (choice.ToLower().Contains(entry.Key.ToLower()))
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

        public static void GetExtraInfo(this object model, string choice, TicketDto t, TicketDetailsMapperModel mapper)
        {
            var productId = GetProductIdByChoice(choice, mapper);
            if (productId != -1)
            {
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
                        ticket.ExtraInfo = new ExtraInfoModel
                        {
                            RefNo = t.TransId.ToString(),
                            TransType = int.Parse(t.GetTransactionType()),
                            ProductId = productId
                        };
                        break;
                    default: break;
                }
            }
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
    }
}