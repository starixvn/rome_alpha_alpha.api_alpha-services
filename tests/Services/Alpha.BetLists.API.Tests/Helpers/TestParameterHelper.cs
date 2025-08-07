using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Models;

namespace Alpha.BetLists.API.Tests.Helpers;

public static class TestParameterHelper
{
    public static GetRunningBetListParam CreateRunningBetListParam(
        int custId = 123,
        string custName = "TestCustomer",
        string siteName = "TestSite",
        IEnumerable<int> productIds = null)
    {
        return new GetRunningBetListParam(
            custId,
            custName,
            siteName,
            productIds ?? new List<int> { 1, 2 });
    }

    public static GetWinLossBetListParam CreateWinLossBetListParam(
        int custId = 123,
        string custName = "TestCustomer",
        string siteName = "TestSite",
        IEnumerable<int> productIds = null)
    {
        var settings = CreateBetListsAppSettings();
        return new GetWinLossBetListParam(
            custId,
            1, // roleId
            "2023-01-01", // fromDate
            "2023-01-31", // toDate
            custName,
            siteName,
            productIds ?? new List<int> { 1, 2 },
            1, // viewerId
            settings);
    }

    public static CasinoDetailsParam CreateCasinoDetailsParam(
        long refNo = 12345,
        int custId = 67890,
        byte transType = 1,
        int productId = 2,
        int pageIndex = 1,
        int pageSize = 10)
    {
        return new CasinoDetailsParam(
            refNo,
            custId,
            transType,
            productId,
            pageIndex,
            pageSize);
    }

    public static BetListsAppSettingsModel CreateBetListsAppSettings(
        string productOfferProductDetail = "1,2,3")
    {
        return new BetListsAppSettingsModel
        {
            ProductOfferProductDetail = productOfferProductDetail
        };
    }
}
