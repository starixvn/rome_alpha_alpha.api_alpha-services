using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class DeleteApiKeyCommand : NonQueryCommand
{
    public DeleteApiKeyCommand(string apiKey, int custId)
    {
        ApiKey = apiKey;
        CustId = custId;
    }

    public string ApiKey { get; }

    public int CustId { get; }

    public override string GetSettingKey() => "Acc_ManageAPI_Del";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(ApiKey) && CustId > 0;
}