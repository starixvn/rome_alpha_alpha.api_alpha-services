using GrpcCustomer;

namespace Alpha.BetLists.API.Services;

public interface IBelongingCheckService
{
    Task<dynamic> CheckBelongingAsync(int viewerId, string custName);
}

public class BelongingCheckService : IBelongingCheckService
{
    private readonly Customer.CustomerClient customerClient;

    public BelongingCheckService(Customer.CustomerClient customerClient)
    {
        this.customerClient = customerClient;
    }

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
