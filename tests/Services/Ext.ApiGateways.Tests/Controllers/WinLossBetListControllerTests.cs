using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using Ext.ApiGateways.Controllers;

namespace Ext.ApiGateways.Tests.Controllers
{
    public class WinLossBetListControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOkResult()
        {
            var service = Substitute.For<IWinLossService>();
            var controller = new WinLossBetListController(service);
            var result = await controller.Get();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
