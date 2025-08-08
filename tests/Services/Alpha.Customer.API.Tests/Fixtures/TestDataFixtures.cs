using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;

namespace Alpha.Customer.API.Tests.Fixtures;

public static class TestDataFixtures
{
    public static CustomerStatus GetSampleCustomerStatus()
    {
        return new CustomerStatus(12345, 0, true, false, false, "[E][H]");
    }

    public static CustomerStatusParam GetSampleCustomerStatusParam()
    {
        return new CustomerStatusParam(12345, 0);
    }

    public static DownlineInfo GetSampleDownlineInfo()
    {
        return new DownlineInfo(67890, 3, false);
    }

    public static CustomerInfoModel GetSampleCustomerInfo()
    {
        return new CustomerInfoModel(12345, 3, "TestSite", 1);
    }

    public static IEnumerable<ProductModel> GetSampleProducts()
    {
        var products = new List<ProductModel>
        {
            new() { Id = 1, Code = "SABA", Name = "SABA Sports" },
            new() { Id = 2, Code = "LIVE", Name = "Live Casino" },
            new() { Id = 3, Code = "SLOT", Name = "Slot Games" },
            new() { Id = 4, Code = "POKER", Name = "Poker Games" }
        };
        return products;
    }

    public static CustomerStatus GetInactiveCustomerStatus()
    {
        return new CustomerStatus(12345, 0, false, false, true, "");
    }

    public static CustomerInfoModel GetMasterLevelCustomerInfo()
    {
        return new CustomerInfoModel(54321, 2, "MasterSite", 2);
    }

    public static CustomerInfoModel GetAgentLevelCustomerInfo()
    {
        return new CustomerInfoModel(98765, 3, "AgentSite", 1);
    }

    public static CustomerInfoModel GetMemberLevelCustomerInfo()
    {
        return new CustomerInfoModel(11111, 4, "MemberSite", 1);
    }
}
