namespace Alpha.BetLists.API.Models;

public class ProductModel
{
    public ProductModel() { }

    public ProductModel(int id, string name, DateTime? endOfSupportDate, int type, int orderNo)
    {
        Id = id;
        Name = name;
        EndOfSupportDate = endOfSupportDate;
        Type = type;
        OrderNo = orderNo;
        TypeName = type == 0 ? string.Empty : name;
    }

    public int Id { get; protected set; }

    public string Name { get; protected set; }

    public DateTime? EndOfSupportDate { get; protected set; }

    public string DisplayName { get; }

    public int Type { get; protected set; }

    public string TypeName { get; protected set; }

    public int OrderNo { get; protected set; }

    public void UpdateProductName(string name)
    {
        Name = name;
        TypeName = name;
    }
}