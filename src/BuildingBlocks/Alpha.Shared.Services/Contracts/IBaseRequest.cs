namespace Alpha.Shared.Services.Contracts;

public interface IBaseRequest
{
    string CustName { get; }
    int ViewerId { get; }
}
