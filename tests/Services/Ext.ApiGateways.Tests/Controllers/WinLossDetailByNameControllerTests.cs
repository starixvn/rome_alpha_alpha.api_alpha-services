using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Ext.ApiGateways.Controllers;

namespace Ext.ApiGateways.Tests.Controllers
{
    public class WinLossDetailByNameControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            var service = Substitute.For<IWinLossDetailService>();
            var controller = new WinLossDetailByNameController(service);
            var result = await controller.Get();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
