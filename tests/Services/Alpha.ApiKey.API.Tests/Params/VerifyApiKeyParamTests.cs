using Alpha.ApiKey.API.Params;
using Xunit;

namespace Alpha.ApiKey.API.Tests.Params
{
    public class VerifyApiKeyParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var key = "test-api-key";
            var passPhrase = "test-passphrase";
            var signature = "test-signature";
            var timeSpan = "test-timespan";

            // Act
            var param = new VerifyApiKeyParam
            {
                Key = key,
                PassPhrase = passPhrase,
                Signature = signature,
                TimeSpan = timeSpan
            };

            // Assert
            Assert.Equal(key, param.Key);
            Assert.Equal(passPhrase, param.PassPhrase);
            Assert.Equal(signature, param.Signature);
            Assert.Equal(timeSpan, param.TimeSpan);
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var param = new VerifyApiKeyParam();

            // Assert
            Assert.Null(param.Key);
            Assert.Null(param.PassPhrase);
            Assert.Null(param.Signature);
            Assert.Null(param.TimeSpan);
        }

        [Fact]
        public void Properties_ShouldHandleEmptyStrings()
        {
            // Act
            var param = new VerifyApiKeyParam
            {
                Key = "",
                PassPhrase = "",
                Signature = "",
                TimeSpan = ""
            };

            // Assert
            Assert.Equal("", param.Key);
            Assert.Equal("", param.PassPhrase);
            Assert.Equal("", param.Signature);
            Assert.Equal("", param.TimeSpan);
        }

        [Fact]
        public void Properties_ShouldHandleNullValues()
        {
            // Act
            var param = new VerifyApiKeyParam
            {
                Key = null,
                PassPhrase = null,
                Signature = null,
                TimeSpan = null
            };

            // Assert
            Assert.Null(param.Key);
            Assert.Null(param.PassPhrase);
            Assert.Null(param.Signature);
            Assert.Null(param.TimeSpan);
        }
    }
}
