namespace Alpha.ApiKey.API.Models;

public record Result(int Code, string Message)
{
    public bool IsFailed => Code > 0;
}