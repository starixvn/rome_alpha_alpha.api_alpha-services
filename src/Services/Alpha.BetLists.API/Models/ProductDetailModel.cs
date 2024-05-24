namespace Alpha.BetLists.API.Models;

public class ProductDetailModel
{
    public string RefId { get; }

    public int ResourceId { get; }

    public int ProductId { get; }

    public int SportType { get; }

    public int BetType { get; }

    public int Order { get; }
    
    public int LeagueGroupId { get; }    

    public int TicketType { get; }

    public override string ToString()
            => string.Format(
                    "<ProductDetail ProductId=\"{0}\" Sporttype=\"{1}\" Bettype=\"{2}\" LeagueGroupId=\"{3}\" TicketType=\"{4}\"/>",
                    ProductId,
                    SportType,
                    BetType,
                    LeagueGroupId,
                    TicketType);

}