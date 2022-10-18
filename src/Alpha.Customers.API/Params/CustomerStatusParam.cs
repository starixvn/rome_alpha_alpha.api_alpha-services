namespace Alpha.Customers.API.Params;

public record CustomerStatusParam(int CustId, int SubAccId)
{
    public string ToXml()
        => $"<r id=\"{CustId}\" sub=\"{SubAccId}\"/>";
}