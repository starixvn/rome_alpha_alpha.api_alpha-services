
using Alpha.Shared.Services.Contracts;

namespace Alpha.Reports.API.Requests;

public record BaseRequest(string CustName, int ViewerId) : IBaseRequest;