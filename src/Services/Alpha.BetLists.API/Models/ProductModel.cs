namespace Alpha.BetLists.API.Models;

public class ProductModel
{
    public int Id { get; protected set; }

    public string Name { get; protected set; }

    public DateTime? EndOfSupportDate { get; protected set; }

    public int Type { get; protected set; }

    public string TypeName { get; protected set; }

    public int OrderNo { get; protected set; }
}