using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class DeleteApiKeyCommand : NonQueryCommand
{
    public DeleteApiKeyCommand(string apiKey, int custId, string ip)
    {
        ApiKey = apiKey;
        CustId = custId;
        IP = ip;
    }

    public string ApiKey { get; }

    public int CustId { get; }

    public string IP { get; }

    public override string GetSettingKey() => "Age_ManageAPI_Del";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(ApiKey) && CustId > 0;
}