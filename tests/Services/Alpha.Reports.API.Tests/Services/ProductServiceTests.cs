using Alpha.Reports.API.Models;
using Alpha.Reports.API.Services;
using Alpha.Utilities.Constants;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.Reports.API.Tests.Services;

public class ProductServiceTests
{
    private readonly IDynamicRepository mockDynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly ICacheService mockCacheService = Substitute.For<ICacheService>();
    private readonly ProductService service;

    public ProductServiceTests()
    {
        service = new ProductService(mockDynamicRepository, mockCacheService);
    }

    [Fact]
    public void FilterProductIdsByCustomer_WithValidProductIds_ReturnsFilteredIds()
    {
        // Arrange
        var inputProductIds = new List<int> { 1, 2, 3, 4 };
        var customerProducts = new List<ProductModel>
        {
            new ProductModel(1, "Product1", null, 1, 1),
            new ProductModel(2, "Product2", null, 2, 2),
            new ProductModel(5, "Product5", null, 5, 5)
        };

        mockCacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(customerProducts);

        // Act
        var result = service.FilterProductIdsByCustomer(inputProductIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void FilterProductIdsByCustomer_WithEmptyProductIds_ReturnsAllCustomerProducts()
    {
        // Arrange
        var inputProductIds = new List<int>();
        var customerProducts = new List<ProductModel>
        {
            new ProductModel(1, "Product1", null, 1, 1),
            new ProductModel(2, "Product2", null, 2, 2)
        };

        mockCacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(customerProducts);

        // Act
        var result = service.FilterProductIdsByCustomer(inputProductIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void FilterProductIdsByCustomer_WithNullProductIds_ReturnsAllCustomerProducts()
    {
        // Arrange
        IEnumerable<int> inputProductIds = null;
        var customerProducts = new List<ProductModel>
        {
            new ProductModel(1, "Product1", null, 1, 1),
            new ProductModel(2, "Product2", null, 2, 2)
        };

        mockCacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(customerProducts);

        // Act
        var result = service.FilterProductIdsByCustomer(inputProductIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void FilterProductIdsByCustomer_WithMinReportDate_FiltersExpiredProducts()
    {
        // Arrange
        var inputProductIds = new List<int> { 1, 2, 3 };
        var minReportDate = DateTime.Now;
        var customerProducts = new List<ProductModel>
        {
            new ProductModel(1, "Product1", DateTime.Now.AddDays(1), 1, 1), // Valid
            new ProductModel(2, "Product2", DateTime.Now.AddDays(-1), 2, 2), // Expired
            new ProductModel(3, "Product3", null, 3, 3) // No end date
        };

        mockCacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(customerProducts);

        // Act
        var result = service.FilterProductIdsByCustomer(inputProductIds, minReportDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new[] { 1, 3 }, result);
    }

    [Fact]
    public void FilterProductIdsByGroup_WithAllGroup_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<int> { 1, 2, 3, 4 };
        var group = ProductGroupConstants.All;

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(products, result);
    }

    [Fact]
    public void FilterProductIdsByGroup_WithEmptyGroup_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<int> { 1, 2, 3, 4 };
        var group = "";

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(products, result);
    }

    [Fact]
    public void FilterProductIdsByGroup_WithNullGroup_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<int> { 1, 2, 3, 4 };
        string group = null;

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(products, result);
    }

    [Fact]
    public void FilterProductIdsByGroup_WithSportsbookGroup_ReturnsSportsbookProducts()
    {
        // Arrange
        var products = new List<int> { ProductIdConstants.Sportsbook, 2, 3, 4 };
        var group = ProductGroupConstants.Sportsbook;

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(ProductIdConstants.Sportsbook, result.First());
    }

    [Fact]
    public void FilterProductIdsByGroup_WithNonSportsbookGroup_ReturnsNonSportsbookProducts()
    {
        // Arrange
        var products = new List<int> { ProductIdConstants.Sportsbook, ProductIdConstants.CashBilling, 3, 4 };
        var group = "NonSportsbook";

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new[] { 3, 4 }, result);
    }

    [Fact]
    public void FilterProductIdsByGroup_WithNullProducts_ReturnsEmpty()
    {
        // Arrange
        IEnumerable<int> products = null;
        var group = ProductGroupConstants.All;

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FilterProductIdsByGroup_WithCaseInsensitiveGroup_HandlesCorrectly()
    {
        // Arrange
        var products = new List<int> { ProductIdConstants.Sportsbook, 2, 3 };
        var group = "SPORTSBOOK"; // Uppercase

        // Act
        var result = service.FilterProductIdsByGroup(products, group);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(ProductIdConstants.Sportsbook, result.First());
    }

    [Fact]
    public void FilterProductIdsByCustomer_CallsCacheService()
    {
        // Arrange
        var inputProductIds = new List<int> { 1, 2, 3 };
        var customerProducts = new List<ProductModel>
        {
            new ProductModel(1, "Product1", null, 1, 1)
        };

        mockCacheService.GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>())
            .Returns(customerProducts);

        // Act
        service.FilterProductIdsByCustomer(inputProductIds);

        // Assert
        mockCacheService.Received(1).GetOrSet(Arg.Any<string>(), Arg.Any<Func<IEnumerable<ProductModel>>>(), Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Arrange & Act
        var service = new ProductService(mockDynamicRepository, mockCacheService);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullDynamicRepository_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new ProductService(null, mockCacheService);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullCacheService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new ProductService(mockDynamicRepository, null);
        Assert.NotNull(service);
    }
}
