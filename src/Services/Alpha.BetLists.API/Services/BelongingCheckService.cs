using GrpcCustomer;

namespace Alpha.BetLists.API.Services;

public interface IBelongingCheckService
{
    Task<dynamic> CheckBelongingAsync(int viewerId, string custName);
}

public class BelongingCheckService(Customer.CustomerClient customerClient) : IBelongingCheckService
{
    private readonly Customer.CustomerClient customerClient = customerClient;
   
    public async Task<dynamic> CheckBelongingAsync(int viewerId, string custName)
    {
        var getDownlineRequest = new GetDownlineInfoRequest
        {
            ViewerId = viewerId,
            DownlineUsername = custName
        };
        return await customerClient.GetDownlineInfoAsync(getDownlineRequest);
    }
}
