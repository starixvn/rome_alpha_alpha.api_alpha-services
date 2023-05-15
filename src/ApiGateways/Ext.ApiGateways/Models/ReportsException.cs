namespace Ext.ApiGateways.Models;

public class ReportsException : Exception
{
    public int Code { get; set; }
    public string Detail { get; set; }
}
