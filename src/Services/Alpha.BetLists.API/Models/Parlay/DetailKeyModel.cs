namespace Alpha.BetLists.API.Models.Parlay
{
    public class DetailKeyModel
    {
        public TicketDto ticket { get; set; }
        public byte ParlayProductId { get; set; }
        public long RefNo { get; set; }
    }
}