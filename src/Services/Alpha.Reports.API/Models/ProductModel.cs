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
        TypeName = type == 0 ? string.Empty : name;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime? EndOfSupportDate { get; set; }

    public int Type { get; set; }

    public string TypeName { get; set; }

    public int OrderNo { get; set; }

    public bool IsHidden { get; set; }
}