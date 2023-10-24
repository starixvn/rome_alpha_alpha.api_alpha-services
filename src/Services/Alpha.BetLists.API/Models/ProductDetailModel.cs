namespace Alpha.BetLists.API.Models;

public class ProductDetailModel
{
    public string RefId { get; }

    public int ResourceId { get; }

    public int ProductId { get; }

    public int SportType { get; }

    public int BetType { get; }

    public int Order { get; }
    public string LeagueGroupId { get; }

    public string[] LeagueGroupIds => LeagueGroupId != null ? LeagueGroupId.Split(',') : Array.Empty<string>();

    public string TicketType { get; }

    public string[] TicketTypeIds => TicketType != null ? TicketType.Split(',') : Array.Empty<string>();
}