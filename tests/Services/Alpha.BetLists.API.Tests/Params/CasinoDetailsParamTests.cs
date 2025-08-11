using Alpha.BetLists.API.Params;
using Xunit;

namespace Alpha.BetLists.API.Tests.Params
{
    public class CasinoDetailsParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAllProperties()
        {
            // Arrange
            var refNo = 123456789L;
            var custId = 100;
            var transType = (byte)1;
            var productId = 200;
            var pageIndex = 0;
            var pageSize = 20;

            // Act
            var param = new CasinoDetailsParam(refNo, custId, transType, productId, pageIndex, pageSize);

            // Assert
            Assert.Equal(refNo, param.RefNo);
            Assert.Equal(custId, param.CustId);
            Assert.Equal(transType, param.TransType);
            Assert.Equal(productId, param.ProductId);
            Assert.Equal(pageIndex, param.PageIndex);
            Assert.Equal(pageSize, param.PageSize);
        }

        [Fact]
        public void Constructor_WithMinimumValues_ShouldHandleCorrectly()
        {
            // Arrange
            var refNo = 0L;
            var custId = 0;
            var transType = (byte)0;
            var productId = 0;
            var pageIndex = 0;
            var pageSize = 0;

            // Act
            var param = new CasinoDetailsParam(refNo, custId, transType, productId, pageIndex, pageSize);

            // Assert
            Assert.Equal(refNo, param.RefNo);
            Assert.Equal(custId, param.CustId);
            Assert.Equal(transType, param.TransType);
            Assert.Equal(productId, param.ProductId);
            Assert.Equal(pageIndex, param.PageIndex);
            Assert.Equal(pageSize, param.PageSize);
        }

        [Fact]
        public void Constructor_WithMaximumValues_ShouldHandleCorrectly()
        {
            // Arrange
            var refNo = long.MaxValue;
            var custId = int.MaxValue;
            var transType = byte.MaxValue;
            var productId = int.MaxValue;
            var pageIndex = int.MaxValue;
            var pageSize = int.MaxValue;

            // Act
            var param = new CasinoDetailsParam(refNo, custId, transType, productId, pageIndex, pageSize);

            // Assert
            Assert.Equal(refNo, param.RefNo);
            Assert.Equal(custId, param.CustId);
            Assert.Equal(transType, param.TransType);
            Assert.Equal(productId, param.ProductId);
            Assert.Equal(pageIndex, param.PageIndex);
            Assert.Equal(pageSize, param.PageSize);
        }

        [Fact]
        public void Constructor_WithNegativeValues_ShouldAcceptThem()
        {
            // Arrange
            var refNo = -1L;
            var custId = -1;
            var transType = (byte)255; // byte can't be negative, so use max value
            var productId = -1;
            var pageIndex = -1;
            var pageSize = -1;

            // Act
            var param = new CasinoDetailsParam(refNo, custId, transType, productId, pageIndex, pageSize);

            // Assert
            Assert.Equal(refNo, param.RefNo);
            Assert.Equal(custId, param.CustId);
            Assert.Equal(transType, param.TransType);
            Assert.Equal(productId, param.ProductId);
            Assert.Equal(pageIndex, param.PageIndex);
            Assert.Equal(pageSize, param.PageSize);
        }
    }
}
