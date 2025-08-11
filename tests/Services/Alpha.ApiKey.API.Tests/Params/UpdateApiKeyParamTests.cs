using Alpha.ApiKey.API.Params;
using Xunit;

namespace Alpha.ApiKey.API.Tests.Params
{
    public class UpdateApiKeyParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var param = new UpdateApiKeyParam();

            // Assert
            Assert.Null(param.ApiKey);
            Assert.Equal(0, param.CustId);
            Assert.Null(param.ApiName);
            Assert.Null(param.Scopes);
            Assert.Null(param.IP);
            Assert.Null(param.Language);
            Assert.Null(param.ExpirationDate);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var param = new UpdateApiKeyParam();
            var apiKey = "test-api-key-123";
            var custId = 456;
            var apiName = "Updated API";
            var scopes = "read,write,delete";
            var ip = "10.0.0.1";
            var language = "fr";
            var expirationDate = DateTime.Now.AddDays(60);

            // Act
            param.ApiKey = apiKey;
            param.CustId = custId;
            param.ApiName = apiName;
            param.Scopes = scopes;
            param.IP = ip;
            param.Language = language;
            param.ExpirationDate = expirationDate;

            // Assert
            Assert.Equal(apiKey, param.ApiKey);
            Assert.Equal(custId, param.CustId);
            Assert.Equal(apiName, param.ApiName);
            Assert.Equal(scopes, param.Scopes);
            Assert.Equal(ip, param.IP);
            Assert.Equal(language, param.Language);
            Assert.Equal(expirationDate, param.ExpirationDate);
        }

        [Fact]
        public void ExpirationDate_ShouldHandleNullValue()
        {
            // Arrange
            var param = new UpdateApiKeyParam();

            // Act
            param.ExpirationDate = null;

            // Assert
            Assert.Null(param.ExpirationDate);
        }

        [Fact]
        public void Properties_ShouldHandleEmptyStrings()
        {
            // Arrange
            var param = new UpdateApiKeyParam();

            // Act
            param.ApiKey = "";
            param.ApiName = "";
            param.Scopes = "";
            param.IP = "";
            param.Language = "";

            // Assert
            Assert.Equal("", param.ApiKey);
            Assert.Equal("", param.ApiName);
            Assert.Equal("", param.Scopes);
            Assert.Equal("", param.IP);
            Assert.Equal("", param.Language);
        }

        [Fact]
        public void Properties_ShouldHandleNullStrings()
        {
            // Arrange
            var param = new UpdateApiKeyParam();

            // Act
            param.ApiKey = null;
            param.ApiName = null;
            param.Scopes = null;
            param.IP = null;
            param.Language = null;

            // Assert
            Assert.Null(param.ApiKey);
            Assert.Null(param.ApiName);
            Assert.Null(param.Scopes);
            Assert.Null(param.IP);
            Assert.Null(param.Language);
        }
    }
}
