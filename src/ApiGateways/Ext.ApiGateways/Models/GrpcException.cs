namespace Ext.ApiGateways.Models;

public class GrpcException : Exception
{
    public int Code { get; set; }
    public string Detail { get; set; }
}
