namespace Alpha.Reports.API.Models;

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
    }

    internal ProductModel(int id, string name, DateTime? endOfSupportDate, int type, int orderNo, Func<int, string> getProductDisplayName)
        : this(id, name, endOfSupportDate, type, orderNo)
    {
        DisplayName = getProductDisplayName == null ? name : getProductDisplayName(id);
        TypeName = type == 0 ? string.Empty : (getProductDisplayName == null ? name : getProductDisplayName(type));
    }

    public int Id { get; protected set; }

    public string Name { get; protected set; }

    public DateTime? EndOfSupportDate { get; protected set; }

    public string DisplayName { get; }

    public int Type { get; protected set; }

    public string TypeName { get; }

    public int OrderNo { get; protected set; }
}