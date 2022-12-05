using Alpha.Utilities.Models;

namespace Alpha.ApiKey.API.Models;

public class ReportsAppSettingsModel : BaseAppSettingsModel
{
    public string GrpcCustomer { get; init; }
    public string RemoteRefURL { get; init; }
}