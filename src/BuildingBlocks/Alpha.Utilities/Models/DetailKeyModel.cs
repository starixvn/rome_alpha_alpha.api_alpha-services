namespace Alpha.Utilities.Models;

public class DetailKeyModel<T>
{
    public T ticket { get; set; }
    public byte ParlayProductId { get; set; }
    public long RefNo { get; set; }
    public bool SupportDetails { get; set; }
}