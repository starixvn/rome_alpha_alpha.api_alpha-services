using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Ext.ApiGateways.Controllers;

namespace Ext.ApiGateways.Tests.Controllers
{
    public class OutstandingControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            // Arrange
            var service = Substitute.For<IOutstandingService>();
            var controller = new OutstandingController(service);

            // Act
            var result = await controller.Get();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
