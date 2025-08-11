using Alpha.ApiKey.API.Params;
using Xunit;

namespace Alpha.ApiKey.API.Tests.Params
{
    public class CreateApiKeyParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var param = new CreateApiKeyParam();

            // Assert
            Assert.Equal(0, param.CustId);
            Assert.Null(param.ApiName);
            Assert.Null(param.PassPhrase);
            Assert.Null(param.Scopes);
            Assert.Null(param.IP);
            Assert.Null(param.Language);
            Assert.Null(param.ExpirationDate);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var param = new CreateApiKeyParam();
            var custId = 123;
            var apiName = "Test API";
            var passPhrase = "test-pass";
            var scopes = "read,write";
            var ip = "192.168.1.1";
            var language = "en";
            var expirationDate = DateTime.Now.AddDays(30);

            // Act
            param.CustId = custId;
            param.ApiName = apiName;
            param.PassPhrase = passPhrase;
            param.Scopes = scopes;
            param.IP = ip;
            param.Language = language;
            param.ExpirationDate = expirationDate;

            // Assert
            Assert.Equal(custId, param.CustId);
            Assert.Equal(apiName, param.ApiName);
            Assert.Equal(passPhrase, param.PassPhrase);
            Assert.Equal(scopes, param.Scopes);
            Assert.Equal(ip, param.IP);
            Assert.Equal(language, param.Language);
            Assert.Equal(expirationDate, param.ExpirationDate);
        }

        [Fact]
        public void ExpirationDate_ShouldHandleNullValue()
        {
            // Arrange
            var param = new CreateApiKeyParam();

            // Act
            param.ExpirationDate = null;

            // Assert
            Assert.Null(param.ExpirationDate);
        }

        [Fact]
        public void Properties_ShouldHandleEmptyStrings()
        {
            // Arrange
            var param = new CreateApiKeyParam();

            // Act
            param.ApiName = "";
            param.PassPhrase = "";
            param.Scopes = "";
            param.IP = "";
            param.Language = "";

            // Assert
            Assert.Equal("", param.ApiName);
            Assert.Equal("", param.PassPhrase);
            Assert.Equal("", param.Scopes);
            Assert.Equal("", param.IP);
            Assert.Equal("", param.Language);
        }
    }
}
