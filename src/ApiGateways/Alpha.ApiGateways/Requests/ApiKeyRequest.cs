using Microsoft.AspNetCore.Mvc;

namespace Alpha.ApiGateways.Requests;

public class ApiKeyRequest
{
    [FromHeader]
    public string Key { get; set; }
    [FromHeader]
    public string PassPhrase { get; set; }
    [FromHeader]
    public string Signature { get; set; }
    [FromHeader]
    public string Timestamp { get; set; }
}