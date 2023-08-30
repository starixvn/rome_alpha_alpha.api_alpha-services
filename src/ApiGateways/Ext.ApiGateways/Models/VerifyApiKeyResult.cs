namespace Ext.ApiGateways.Models;

public class VerifyApiKeyResult
{
    public bool IsAuthenticated { get; set; }
    public int CustId { get; set; }
    public int SubAccId { get; set; }
}
