using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetDownlineInfoCommand : NonQueryCommand
{
    public GetDownlineInfoCommand(int uplineId, int subAccId, string username)
    {
        UplineId = uplineId;
        SubAccountId = subAccId;
        Username = username;
    }

    public int UplineId { get; }

    public int SubAccountId { get; }

    public string Username { get; }

    public bool IsActive { get; set; }

    public int CustId { get; set; }

    public bool IsSubAcc { get; set; }

    public int RoleId { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelCheckBelonging_ByUsername";

    public override bool IsValid()
        => UplineId > 0 && SubAccountId >= 0 && !string.IsNullOrWhiteSpace(Username);
}
