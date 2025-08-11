using Alpha.BetLists.API.Models;
using Xunit;

namespace Alpha.BetLists.API.Tests.Models
{
    public class BetlistOutputParamsTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var output = new BetlistOutputParams();

            // Assert
            Assert.Equal(0, output.ErrCode);
            Assert.Null(output.ErrMsg);
            Assert.Equal(0, output.MaxTotalRow);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldInitializeAllProperties()
        {
            // Arrange
            var errCode = 404;
            var errMsg = "Not Found";
            var maxTotalRow = 1000;

            // Act
            var output = new BetlistOutputParams(errCode, errMsg, maxTotalRow);

            // Assert
            Assert.Equal(errCode, output.ErrCode);
            Assert.Equal(errMsg, output.ErrMsg);
            Assert.Equal(maxTotalRow, output.MaxTotalRow);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var output = new BetlistOutputParams();
            var errCode = 200;
            var errMsg = "Success";
            var maxTotalRow = 500;

            // Act
            output.ErrCode = errCode;
            output.ErrMsg = errMsg;
            output.MaxTotalRow = maxTotalRow;

            // Assert
            Assert.Equal(errCode, output.ErrCode);
            Assert.Equal(errMsg, output.ErrMsg);
            Assert.Equal(maxTotalRow, output.MaxTotalRow);
        }

        [Fact]
        public void Constructor_WithNullErrorMessage_ShouldHandleCorrectly()
        {
            // Act
            var output = new BetlistOutputParams(500, null, 0);

            // Assert
            Assert.Equal(500, output.ErrCode);
            Assert.Null(output.ErrMsg);
            Assert.Equal(0, output.MaxTotalRow);
        }

        [Fact]
        public void Constructor_WithEmptyErrorMessage_ShouldHandleCorrectly()
        {
            // Act
            var output = new BetlistOutputParams(0, "", 100);

            // Assert
            Assert.Equal(0, output.ErrCode);
            Assert.Equal("", output.ErrMsg);
            Assert.Equal(100, output.MaxTotalRow);
        }

        [Fact]
        public void Constructor_WithNegativeValues_ShouldAcceptThem()
        {
            // Act
            var output = new BetlistOutputParams(-1, "Error", -100);

            // Assert
            Assert.Equal(-1, output.ErrCode);
            Assert.Equal("Error", output.ErrMsg);
            Assert.Equal(-100, output.MaxTotalRow);
        }
    }
}
