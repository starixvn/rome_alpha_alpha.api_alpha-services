namespace Alpha.Utilities.Models;

public class GrpcException : Exception
{
    public int Code { get; set; }
    public string Detail { get; set; }
}
