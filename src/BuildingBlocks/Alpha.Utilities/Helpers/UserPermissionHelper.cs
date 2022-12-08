using Alpha.Utilities.Constans;
using Alpha.Utilities.Models;

namespace Alpha.Utilities.Helpers;

public static class UserPermissionHelper
{
    private static readonly string neutralSite = "789y,"; // Seperate by comma
    private static readonly string creditNeutralSite = "789y,"; // Seperate by comma

    public static bool IsNeutral(string site, BaseAppSettingsModel settings)
        => settings.IsNeutralSite || (site != null && neutralSite.IndexOf(site, 0, StringComparison.CurrentCultureIgnoreCase) != -1);

    public static bool IsCreditNeutral(string site)
        => site != null && creditNeutralSite.IndexOf(site, 0, StringComparison.CurrentCultureIgnoreCase) != -1;

    public static bool IsShowNumberGame(string site, BaseAppSettingsModel settings)
        => (!IsNeutral(site, settings) || IsCreditNeutral(site)) && !settings.DisabledNumberGame;

    public static bool IsShowKenoSetting(IEnumerable<ProductStatusModel> productStatus, BaseAppSettingsModel settings)
        => settings.IsEnableKeno && IsShow(ProductIdConstants.Keno, productStatus, false);

    public static bool IsShowSportsbook2(IEnumerable<ProductStatusModel> productStatus, BaseAppSettingsModel settings)
        => settings.IsShowSportsbook2 && IsShow(ProductIdConstants.Sportsbook2, productStatus, false);

    public static bool IsEnableProduct(int productId, IEnumerable<ProductStatusModel> productStatus, BaseAppSettingsModel settings)
        => settings.IsEnabledProduct(productId) && IsShow(productId, productStatus, false);

    public static bool IsSyncedProduct(int productId, IEnumerable<ProductStatusModel> productStatus, BaseAppSettingsModel settings)
        => settings.IsEnabledProduct(productId) && IsSynced(productId, productStatus, false);

    public static bool IsShow(int productId, IEnumerable<ProductStatusModel> productStatus, bool defaultValue)
    {
        var p = productStatus.FirstOrDefault(p => p.Id == productId);
        return p == null ? defaultValue : !p.IsHidden;
    }

    public static bool IsSynced(int productId, IEnumerable<ProductStatusModel> productStatus, bool defaultValue)
    {
        var p = productStatus.FirstOrDefault(p => p.Id == productId);
        return p == null ? defaultValue : p.IsSynced;
    }

    public static bool HasPermissionOnProduct(int productId, string site, IEnumerable<ProductStatusModel> productStatus, BaseAppSettingsModel settings)
        => productId switch
        {
            ProductIdConstants.Sportsbook => true,
            ProductIdConstants.NumberGame => IsShowNumberGame(site, settings),
            ProductIdConstants.Keno => IsShowKenoSetting(productStatus, settings),
            ProductIdConstants.Sportsbook2 => IsShowSportsbook2(productStatus, settings),
            ProductIdConstants.VirtualSports or ProductIdConstants.RNGLottery => IsEnableProduct(productId, productStatus, settings),
            _ => IsEnableProduct(productId, productStatus, settings) && IsSyncedProduct(productId, productStatus, settings),
        };
}