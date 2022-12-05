
using Alpha.Customer.API.Criteria;
using Alpha.Customer.API.Models;
using Fanex.Data;

namespace Alpha.Customer.API.DataAccesses;

public interface ICustomerDataAccess
{
    Task<CustomerInfoModel> GetCustomerInfo(GetCustomerInfoCriteria criteria);
    Task<IEnumerable<ProductModel>> GetCustomerProducts(GetCustomerProductsCriteria criteria);
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

            if (reader != null)
            {
                while (reader.Read())
                {
                    var custId = (int)reader["custid"];
                    var roleId = (byte)reader["roleid"];
                    var site = reader["site"].ToString();
                    var currency = (int)reader["currency"];
                    return new CustomerInfoModel(custId, roleId, site, currency);
                }
            }
        }

        return new CustomerInfoModel(0, 0, string.Empty, 0);
    }

    public async Task<IEnumerable<ProductModel>> GetCustomerProducts(GetCustomerProductsCriteria criteria)
    {
        if (!criteria.IsValid())
        {
            return Enumerable.Empty<ProductModel>();
        }

        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            using var reader = await db.ExecuteReaderAsync(criteria);
            reader.Read();
            if (reader.NextResult())
            {
                var result = new List<ProductModel>();

                while (reader.Read())
                {
                    var productId = (byte)reader["ProductID"];
                    var isSync = (bool)reader["IsSync"];
                    var isHidden = (bool)reader["isHidden"];
                    var product = new ProductModel(productId, isSync, isHidden);
                    result.Add(product);
                }

                return result;
            }
        }

        return Enumerable.Empty<ProductModel>();
    }
}