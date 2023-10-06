using System.Text;
using Alpha.Customer.API.Params;
using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetCustomerStatusCriteria : CriteriaBase
{
    public GetCustomerStatusCriteria(IEnumerable<CustomerStatusParam> custList)
        => CustList = BuildCustListParam(custList);

    public string CustList { get; }

    public override string GetSettingKey() => "Age_Chatbot_CustomerStatusSel";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(CustList);

    private static string BuildCustListParam(IEnumerable<CustomerStatusParam> customers)
    {
        var spParams = new StringBuilder();
        spParams.Append("<Root>");

        foreach (var item in customers)
        {
            spParams.Append(item.ToXml());
        }

        spParams.Append("</Root>");
        return spParams.ToString();
    }
}
