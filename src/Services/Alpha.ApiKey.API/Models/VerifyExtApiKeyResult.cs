namespace Alpha.ApiKey.API.Models;

public class VerifyExtApiKeyResult
{
    public bool IsAuthenticated { get; set; }
    public int CustId { get; set; }
    public int SubAccId { get; set; }
    public string ErrMsg { get; set; }
    public string Site { get; set; }
    public int Currency { get; set; }
}