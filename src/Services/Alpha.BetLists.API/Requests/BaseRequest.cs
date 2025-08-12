
using Alpha.Shared.Services.Contracts;

namespace Alpha.BetLists.API.Requests;

public record BaseRequest(string CustName, int ViewerId) : IBaseRequest;