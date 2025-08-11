using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Ext.ApiGateways.Controllers;

namespace Ext.ApiGateways.Tests.Controllers
{
    public class ParlayDetailsControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            var service = Substitute.For<IParlayService>();
            var controller = new ParlayDetailsController(service);
            var result = await controller.Get();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
