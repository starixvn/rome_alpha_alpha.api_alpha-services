using Alpha.ApiKey.API.Models;

namespace Alpha.ApiKey.API.Tests.Fixtures;

public static class TestDataFixtures
{
    public static class ApiKeyEntities
    {
        public static ApiKeyEntity ValidApiKey => new()
        {
            CustId = 12345,
            RoleId = 3,
            Name = "Test API Key",
            Key = "abc123def456",
            Secret = "secret123456789",
            PassPhrase = "hashedPassPhrase",
            ScopeIdList = "1,2,3",
            CreatedDate = DateTime.Now.AddDays(-1),
            ExpirationDate = DateTime.Now.AddDays(30)
        };

        public static ApiKeyEntity ExpiredApiKey => new()
        {
            CustId = 12345,
            RoleId = 3,
            Name = "Expired API Key",
            Key = "expired123",
            Secret = "expiredsecret",
            PassPhrase = "hashedPassPhrase",
            ScopeIdList = "1,2",
            CreatedDate = DateTime.Now.AddDays(-60),
            ExpirationDate = DateTime.Now.AddDays(-1)
        };

        public static ApiKeyEntity NonExpiringApiKey => new()
        {
            CustId = 67890,
            RoleId = 1,
            Name = "Non-Expiring API Key",
            Key = "nonexpiring123",
            Secret = "nonexpiringsecret",
            PassPhrase = "hashedPassPhrase",
            ScopeIdList = "1,2,3,4",
            CreatedDate = DateTime.Now.AddDays(-10),
            ExpirationDate = null
        };
    }

    public static class ApiKeyResults
    {
        public static ApiKeyResult ValidResult => new("Test API Key", "abc123def456", "secret123456789");
    }

    public static class CreateApiKeyResults
    {
        public static CreateApiKeyResult SuccessResult => new()
        {
            ApiKeyResult = ApiKeyResults.ValidResult,
            Result = null
        };

        public static CreateApiKeyResult ErrorResult => new()
        {
            ApiKeyResult = null,
            Result = new Result(1001, "API Key creation failed")
        };
    }

    public static class VerifyApiKeyResults
    {
        public static VerifyApiKeyResult ValidResult => new(true)
        {
            CustId = 12345,
            RoleId = 3,
            Scopes = "1,2,3"
        };

        public static VerifyApiKeyResult InvalidResult => new(false, "Invalid signature");

        public static VerifyApiKeyResult ExpiredResult => new(false, "Time expired!");
    }

    public static class AppSettings
    {
        public static APIKeyAppSettings DefaultSettings => new()
        {
            ExpireSignatureTime = 300000, // 5 minutes
            AllowSignatureTimeTimeRange = 60000 // 1 minute
        };
    }
}
