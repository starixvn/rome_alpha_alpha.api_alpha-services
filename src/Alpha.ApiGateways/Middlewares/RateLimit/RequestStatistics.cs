namespace Alpha.ApiGateways.Middlewares.RateLimit;

public class RequestStatistics
{
    public DateTime LastSuccessfulResponseTime { get; set; }

    public int TotalRequestOfKey { get; set; }

    public int TotalRequestOfCustId { get; set; }
}
