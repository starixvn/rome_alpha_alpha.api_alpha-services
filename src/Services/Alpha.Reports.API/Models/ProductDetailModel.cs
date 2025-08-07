namespace Alpha.Reports.API.Models;

public class ProductDetailModel
{
    public ProductDetailModel() {}
    public ProductDetailModel(int productId, int resourceId)
    {
        ProductId = productId;
        ResourceId = resourceId;
    }

    public string RefId { get; set; }
    public int ResourceId { get; set; }
    public int ProductId { get; set; }
    public int SportType { get; set; }
    public int BetType { get; set; }
    public int Order { get; set; }
    public int LeagueGroupId { get; set; }
    public int TicketType { get; set; }

    public override string ToString()
            => string.Format(
                    "<ProductDetail ProductId=\"{0}\" Sporttype=\"{1}\" Bettype=\"{2}\" LeagueGroupId=\"{3}\" TicketType=\"{4}\"/>",
                    ProductId,
                    SportType,
                    BetType,
                    LeagueGroupId,
                    TicketType);
}