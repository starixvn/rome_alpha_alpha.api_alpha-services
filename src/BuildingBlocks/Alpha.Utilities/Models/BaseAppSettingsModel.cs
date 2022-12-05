namespace Alpha.Utilities.Models;

public class BaseAppSettingsModel
{
    public byte SettingStatus { get; init; }
    public bool IsNeutralSite { get; init; }
    public bool DisabledNumberGame { get; init; }
    public bool IsEnableKeno { get; init; }
    public bool IsShowSportsbook2 { get; init; }

    public IEnumerable<int> EnableProducts { get; init; }

    public bool IsEnabledProduct(int productId)
        => EnableProducts != null && EnableProducts.Any(p => p == productId);
}