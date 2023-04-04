namespace Alpha.ApiKey.API.Models;

public class CreateApiKeyResult
{
    public ApiKeyResult ApiKeyResult { get; set; } = default;

    public Result Result { get; set; } = default;
}