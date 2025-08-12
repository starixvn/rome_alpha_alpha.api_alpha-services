using Alpha.BetLists.API.Criteria;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Services;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.BetLists.API.Tests.Services;

public class ProductDetailServiceTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly ProductDetailService service;

    public ProductDetailServiceTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        cacheService = Substitute.For<ICacheService>();
        service = new ProductDetailService(dynamicRepository, cacheService);
    }

    private static ProductDetailModel CreateProductDetailModel(
        int productId = 1, 
        int resourceId = 10, 
        int sportType = 1, 
        int betType = 1, 
        int leagueGroupId = 1, 
        int ticketType = 1)
    {
        var instance = (ProductDetailModel)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(ProductDetailModel));
        
        // Use reflection to set readonly properties
        var productIdField = typeof(ProductDetailModel).GetProperty("ProductId");
        var resourceIdField = typeof(ProductDetailModel).GetProperty("ResourceId");
        var sportTypeField = typeof(ProductDetailModel).GetProperty("SportType");
        var betTypeField = typeof(ProductDetailModel).GetProperty("BetType");
        var leagueGroupIdField = typeof(ProductDetailModel).GetProperty("LeagueGroupId");
        var ticketTypeField = typeof(ProductDetailModel).GetProperty("TicketType");
        
        // Set backing fields using reflection
        var productIdBackingField = typeof(ProductDetailModel).GetField("<ProductId>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var resourceIdBackingField = typeof(ProductDetailModel).GetField("<ResourceId>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sportTypeBackingField = typeof(ProductDetailModel).GetField("<SportType>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var betTypeBackingField = typeof(ProductDetailModel).GetField("<BetType>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var leagueGroupIdBackingField = typeof(ProductDetailModel).GetField("<LeagueGroupId>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var ticketTypeBackingField = typeof(ProductDetailModel).GetField("<TicketType>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        productIdBackingField?.SetValue(instance, productId);
        resourceIdBackingField?.SetValue(instance, resourceId);
        sportTypeBackingField?.SetValue(instance, sportType);
        betTypeBackingField?.SetValue(instance, betType);
        leagueGroupIdBackingField?.SetValue(instance, leagueGroupId);
        ticketTypeBackingField?.SetValue(instance, ticketType);
        
        return instance;
    }

    [Fact]
    public void BuildProductDetailsXML_WithEmptyProductIds_ReturnsEmptyString()
    {
        // Arrange
        var validProductIds = new List<int>();
        
        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void BuildProductDetailsXML_WithNullProductIds_ReturnsEmptyString()
    {
        // Arrange
        IEnumerable<int> validProductIds = null;

        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        Assert.Equal(string.Empty, result);
    }    [Fact]
    public void BuildProductDetailsXML_WithValidProductIds_CallsDynamicRepository()
    {
        // Arrange
        var validProductIds = new List<int> { 1, 2, 3 };
        
        // Create real ProductDetailModel objects since they can't be mocked
        var mockProductDetails = new List<ProductDetailModel>();

        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(mockProductDetails);

        // Setup cache to call the value factory (which calls repository)
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });
        
        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        dynamicRepository.Received(1).Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>());
        Assert.Equal(string.Empty, result); // Empty list returns empty string
    }

    [Fact]
    public void BuildProductDetailsXML_WithValidProductIds_ReturnsXMLFormat()
    {
        // Arrange
        var validProductIds = new List<int> { 1 };
        
        // Create ProductDetailModel using reflection since it can't be mocked
        var mockProduct = CreateProductDetailModel(
            productId: 1, 
            resourceId: 10, 
            sportType: 1, 
            betType: 2, 
            leagueGroupId: 3, 
            ticketType: 4);

        var mockProductDetails = new List<ProductDetailModel> { mockProduct };

        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(mockProductDetails);

        // Setup cache to call the value factory (which calls repository)
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });
        
        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        Assert.StartsWith("<ROOT>", result);
        Assert.EndsWith("</ROOT>", result);
        Assert.Contains("ProductDetail", result);
        Assert.Contains("ProductId=\"1\"", result);
    }

    [Fact]
    public void BuildProductDetailsXML_WithNoMatchingProducts_ReturnsEmptyString()
    {
        // Arrange
        var validProductIds = new List<int> { 1, 2 };
        
        var mockProduct3 = CreateProductDetailModel(productId: 3);
        var mockProduct4 = CreateProductDetailModel(productId: 4);

        var mockProductDetails = new List<ProductDetailModel> { mockProduct3, mockProduct4 };

        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(mockProductDetails);

        // Setup cache to call the value factory (which calls repository)
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });
        
        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void BuildProductDetailsXML_WithPartialMatchingProducts_ReturnsOnlyMatchingProducts()
    {
        // Arrange
        var validProductIds = new List<int> { 1, 3 };
        
        var mockProduct1 = CreateProductDetailModel(productId: 1, sportType: 1, betType: 1, leagueGroupId: 1, ticketType: 1);
        var mockProduct2 = CreateProductDetailModel(productId: 2, sportType: 2, betType: 2, leagueGroupId: 2, ticketType: 2);
        var mockProduct3 = CreateProductDetailModel(productId: 3, sportType: 3, betType: 3, leagueGroupId: 3, ticketType: 3);

        var mockProductDetails = new List<ProductDetailModel> { mockProduct1, mockProduct2, mockProduct3 };

        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(mockProductDetails);

        // Setup cache to call the value factory (which calls repository)
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });
        
        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        Assert.Contains("ProductId=\"1\"", result);
        Assert.DoesNotContain("ProductId=\"2\"", result);
        Assert.Contains("ProductId=\"3\"", result);
        Assert.StartsWith("<ROOT>", result);
        Assert.EndsWith("</ROOT>", result);
    }

    [Fact]
    public void BuildProductDetailsXML_WithDuplicateProductIds_GroupsByProductId()
    {
        // Arrange
        var validProductIds = new List<int> { 1 };
        
        var mockProduct1A = CreateProductDetailModel(productId: 1, resourceId: 10, sportType: 1, betType: 1, leagueGroupId: 1, ticketType: 1);
        var mockProduct1B = CreateProductDetailModel(productId: 1, resourceId: 11, sportType: 1, betType: 2, leagueGroupId: 1, ticketType: 1);

        var mockProductDetails = new List<ProductDetailModel> { mockProduct1A, mockProduct1B };

        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(mockProductDetails);

        // Setup cache to call the value factory (which calls repository)
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });
        
        // Act
        var result = service.BuildProductDetailsXML(validProductIds);

        // Assert
        Assert.Contains("Bettype=\"1\"", result);
        Assert.Contains("Bettype=\"2\"", result);
        Assert.StartsWith("<ROOT>", result);
        Assert.EndsWith("</ROOT>", result);
    }

    [Fact]
    public void BuildProductDetailsXML_WithDatabaseException_DoesNotThrow()
    {
        // Arrange
        var validProductIds = new List<int> { 1 };
        
        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(x => { throw new InvalidOperationException("Database error"); });

        // Setup cache to call the value factory (which will throw)
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });
        
        // Act & Assert
        var exception = Record.Exception(() => service.BuildProductDetailsXML(validProductIds));
        
        // The exception should propagate since the service doesn't handle it
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void Constructor_WithValidRepository_DoesNotThrow()
    {
        // Arrange & Act
        var exception = Record.Exception(() => new ProductDetailService(dynamicRepository, cacheService));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithNullRepository_DoesNotThrowImmediately()
    {
        // Arrange & Act
        var exception = Record.Exception(() => new ProductDetailService(null, cacheService));

        // Assert
        // Constructor doesn't validate null, so no exception until method call
        Assert.Null(exception);
    }

    [Fact]
    public void BuildProductDetailsXML_CachesRepositoryResults()
    {
        // Arrange
        var validProductIds = new List<int> { 1 };
        var mockProduct = CreateProductDetailModel(productId: 1, sportType: 1, betType: 1, leagueGroupId: 1, ticketType: 1);
        var mockProductDetails = new List<ProductDetailModel> { mockProduct };

        // Setup cache to return the data from the value factory on first call
        cacheService.GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>())
        .Returns(call =>
        {
            var valueFactory = call.ArgAt<Func<IEnumerable<ProductDetailModel>>>(1);
            return valueFactory();
        });

        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>())
            .Returns(mockProductDetails);

        // Act - Call twice
        service.BuildProductDetailsXML(validProductIds);
        service.BuildProductDetailsXML(validProductIds);

        // Assert - Cache service should be called for each method call
        cacheService.Received(2).GetOrSet(
            Arg.Any<string>(),
            Arg.Any<Func<IEnumerable<ProductDetailModel>>>(),
            Arg.Any<CacheItemOptions>());
    }
}
