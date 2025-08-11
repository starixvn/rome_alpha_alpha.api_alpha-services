using Alpha.Customer.API.Params;
using Xunit;

namespace Alpha.Customer.API.Tests.Params
{
    public class CustomerStatusParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var custId = 123;
            var subAccId = 456;

            // Act
            var param = new CustomerStatusParam(custId, subAccId);

            // Assert
            Assert.Equal(custId, param.CustId);
            Assert.Equal(subAccId, param.SubAccId);
        }

        [Fact]
        public void ToXml_ShouldReturnCorrectXmlFormat()
        {
            // Arrange
            var custId = 123;
            var subAccId = 456;
            var param = new CustomerStatusParam(custId, subAccId);
            var expectedXml = "<r id=\"123\" sub=\"456\"/>";

            // Act
            var result = param.ToXml();

            // Assert
            Assert.Equal(expectedXml, result);
        }

        [Fact]
        public void ToXml_WithZeroValues_ShouldReturnCorrectXmlFormat()
        {
            // Arrange
            var custId = 0;
            var subAccId = 0;
            var param = new CustomerStatusParam(custId, subAccId);
            var expectedXml = "<r id=\"0\" sub=\"0\"/>";

            // Act
            var result = param.ToXml();

            // Assert
            Assert.Equal(expectedXml, result);
        }

        [Fact]
        public void ToXml_WithNegativeValues_ShouldReturnCorrectXmlFormat()
        {
            // Arrange
            var custId = -1;
            var subAccId = -2;
            var param = new CustomerStatusParam(custId, subAccId);
            var expectedXml = "<r id=\"-1\" sub=\"-2\"/>";

            // Act
            var result = param.ToXml();

            // Assert
            Assert.Equal(expectedXml, result);
        }

        [Fact]
        public void RecordEquality_ShouldWorkCorrectly()
        {
            // Arrange
            var param1 = new CustomerStatusParam(123, 456);
            var param2 = new CustomerStatusParam(123, 456);
            var param3 = new CustomerStatusParam(123, 789);

            // Act & Assert
            Assert.Equal(param1, param2);
            Assert.NotEqual(param1, param3);
            Assert.True(param1 == param2);
            Assert.False(param1 == param3);
        }
    }
}
