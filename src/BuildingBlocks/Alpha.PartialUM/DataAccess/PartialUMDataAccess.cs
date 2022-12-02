using System.Data;
using Alpha.PartialUM.Models;
using Alpha.Reports.API.Criteria;
using Fanex.Data;

namespace Alpha.PartialUM.Services;

public interface IPartialUMDataAccess
{
    IEnumerable<UMInfoModel> GetUMInfoList(string market);
}

public class PartialUMDataAccess : IPartialUMDataAccess
{
    public IEnumerable<UMInfoModel> GetUMInfoList(string market)
    {
        var criteria = new GetUMInfoListCriteria(market);
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var reader = db.ExecuteReader(criteria);

        while (reader.Read())
        {
            yield return ToUMInfo(reader);
        }
    }

    private static UMInfoModel ToUMInfo(IDataReader reader)
    {
        return new UMInfoModel(
                reader["FunctionTypeName"]?.ToString() ?? string.Empty,
                Convert.ToDateTime(reader["BeginTime"]),
                Convert.ToDateTime(reader["EndTime"]));
    }
}