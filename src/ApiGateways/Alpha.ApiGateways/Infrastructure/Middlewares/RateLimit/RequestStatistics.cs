namespace Alpha.ApiGateways.Middlewares.RateLimit;

public class RequestStatistics
{
    public DateTime EndTime { get; set; }

    public int TotalRequestOfKey { get; set; }
}
