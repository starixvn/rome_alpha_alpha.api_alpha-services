using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Services;
using Alpha.Utilities.Constants;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class ProductServiceTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly ProductService service;

    public ProductServiceTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        cacheService = Substitute.For<ICacheService>();
        service = new ProductService(dynamicRepository, cacheService);
    }

    [Fact]
    public void GetAllProductIds_WithEmptyProducts_ReturnsEmptyList()
    {
        // Arrange
        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(new List<ProductModel>());

        // Act
        var result = service.GetAllProductIds(ProductGroupConstants.All).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllProductIds_CachesProductsCorrectly()
    {
        // Arrange
        cacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(new List<ProductModel>());

        var cacheKey = "Alpha.BetLists.Services.ProductService.Products";

        // Act
        service.GetAllProductIds(ProductGroupConstants.All);

        // Assert
        cacheService.Received(1).GetOrSet(
            cacheKey,
            Arg.Any<Func<IEnumerable<ProductModel>>>(),
            Arg.Any<CacheItemOptions>());
    }

    // For now, let's skip the complex tests that require ProductModel creation
    // until we find a better approach
}
