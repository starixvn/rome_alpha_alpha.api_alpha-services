namespace Ext.ApiGateways.Requests;

public class ApiKeyRequest
{
    public string Key { get; set; }

    public string PassPhrase { get; set; }

    public string Signature { get; set; }

    public string Timestamp { get; set; }
}