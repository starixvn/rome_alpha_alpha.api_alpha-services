using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public class GetUMInfoListCriteria : CriteriaBase
{
    public GetUMInfoListCriteria(string market)
    {
        MartketName = market;
    }

    public string MartketName { get; }

    public override string GetSettingKey()
        => "Acc_CheckUM_ByReport";

    public override bool IsValid() => !string.IsNullOrEmpty(MartketName);
}