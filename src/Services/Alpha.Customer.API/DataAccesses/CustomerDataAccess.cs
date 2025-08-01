using Alpha.Customer.API.Criteria;
using Alpha.Customer.API.Models;
using Fanex.Data;

namespace Alpha.Customer.API.DataAccesses;

public interface ICustomerDataAccess
{
    Task<CustomerInfoModel> GetCustomerInfo(GetCustomerInfoCriteria criteria);
}


public class CustomerDataAccess : ICustomerDataAccess
{
    public async Task<CustomerInfoModel> GetCustomerInfo(GetCustomerInfoCriteria criteria)
    {
        if (!criteria.IsValid())
        {
            return new CustomerInfoModel(0, 0, string.Empty, 0);
        }

        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            using var reader = await db.ExecuteReaderAsync(criteria);

            if (reader != null && reader.Read())
            {
                var custId = (int)reader["custid"];
                var roleId = (byte)reader["roleid"];
                var site = reader["site"].ToString();
                var currency = int.Parse((reader["currency"] ?? string.Empty).ToString());
                return new CustomerInfoModel(custId, roleId, site, currency);
            }
        }

        return new CustomerInfoModel(0, 0, string.Empty, 0);
    }
}