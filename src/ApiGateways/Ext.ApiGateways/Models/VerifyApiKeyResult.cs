namespace Ext.ApiGateways.Models;

public class VerifyApiKeyResult
{
    public bool IsAuthenticated { get; set; }
    public int CustId { get; set; }
    public int SubAccId { get; set; }
    public string ErrMsg { get; set; }
    public string SiteName { get; set; }
    public int Currency { get; set; }
}
