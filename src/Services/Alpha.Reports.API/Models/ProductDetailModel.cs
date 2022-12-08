namespace Alpha.Reports.API.Models;

public class ProductDetailModel
{
    public ProductDetailModel() { }

    public ProductDetailModel(
        string refId,
        int resourceId,
        int productId,
        int sportType,
        int betType,
        int order,
        string leagueGroupId,
        string ticketType)
    {
        RefId = refId;
        ResourceId = resourceId;
        ProductId = productId;
        SportType = sportType;
        BetType = betType;
        Order = order;
        LeagueGroupId = leagueGroupId;
        TicketType = ticketType;
    }

    public string RefId { get; }

    public int ResourceId { get; }

    public int ProductId { get; }

    public int SportType { get; }

    public int BetType { get; }

    public int Order { get; }

    public string DisplayName { get; set; }

    public string LeagueGroupId { get; }

    public string[] LeagueGroupIds => LeagueGroupId != null ? LeagueGroupId.Split(',') : Array.Empty<string>();

    public string TicketType { get; }

    public string[] TicketTypeIds => TicketType != null ? TicketType.Split(',') : Array.Empty<string>();
}