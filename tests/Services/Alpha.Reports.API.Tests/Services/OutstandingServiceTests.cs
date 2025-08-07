using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Services;
using Alpha.PartialUM.Services;
using Fanex.Data.Repository;
using NSubstitute;
using Xunit;

namespace Alpha.Reports.API.Tests.Services;

public class OutstandingServiceTests
{
    private readonly IDynamicRepository dynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly IPartialUMService partialUMService = Substitute.For<IPartialUMService>();
    private readonly OutstandingService service;

    public OutstandingServiceTests()
    {
        service = new OutstandingService(dynamicRepository, partialUMService);
    }

    [Fact]
    public async Task GetReport_InvalidParam_ReturnsEmpty()
    {
        // Arrange
        var param = new GetOutstandingReportParam(1, 1, 0, "1,2", false, "site", 1); // CustId = 0 is invalid

        // Act
        var result = await service.GetReport(param);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetReport_ValidParam_ReturnsData()
    {
        // Arrange
        var param = new GetOutstandingReportParam(1, 2, 1, "1,2", false, "site", 1);
        var expected = new List<OutstandingModel> { new OutstandingModel { CustId = 1, CustName = "Test" } };
        dynamicRepository.FetchAsync<OutstandingModel>(Arg.Any<ICriteria>()).Returns(expected);

        // Act
        var result = await service.GetReport(param);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(expected.First().CustId, result.First().CustId);
        Assert.Equal(expected.First().CustName, result.First().CustName);
    }
}
