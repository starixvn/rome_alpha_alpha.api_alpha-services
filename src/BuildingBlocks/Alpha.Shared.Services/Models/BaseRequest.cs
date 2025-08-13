using Alpha.Shared.Services.Contracts;

namespace Alpha.Shared.Services.Models;

public class BaseRequest : IBaseRequest
{
    public string CustName { get; set; } = string.Empty;
    public int ViewerId { get; set; }
}
