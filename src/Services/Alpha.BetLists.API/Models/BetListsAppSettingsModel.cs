using Alpha.Utilities.Models;

namespace Alpha.BetLists.API.Models;

public class BetListsAppSettingsModel : BaseAppSettingsModel
{
    public string GrpcCustomer { get; init; }
}