using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Services;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.Reports.API.Tests.Services;

public class ProductDetailServiceTests
{
    private readonly IDynamicRepository dynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly ProductDetailService service;

    public ProductDetailServiceTests()
    {
        service = new ProductDetailService(dynamicRepository);
    }

    [Fact]
    public void GetProductDetails_WithValidSettings_ReturnsFilteredProducts()
    {
        // Arrange
        var settings = new ReportsAppSettingsModel { ProductOfferProductDetail = "1,2" };
        var allProducts = new List<ProductDetailModel>
        {
            new ProductDetailModel { ProductId = 1, ResourceId = 10 },
            new ProductDetailModel { ProductId = 2, ResourceId = 20 },
            new ProductDetailModel { ProductId = 3, ResourceId = 30 }
        };
        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>()).Returns(allProducts);

        // Act
        var result = service.GetProductDetails(settings).ToList();

        // Assert
        Assert.Contains(result, p => p.ProductId == 1);
        Assert.Contains(result, p => p.ProductId == 2);
        Assert.DoesNotContain(result, p => p.ProductId == 3);
    }

    [Fact]
    public void BuildProductDetailsXML_WithValidInputs_ReturnsXml()
    {
        // Arrange
        var validProductIds = new List<int> { 1 };
        var validResourceIds = new List<int> { 10 };
        var productOfferProductDetail = new List<int> { 1 };
        var allProducts = new List<ProductDetailModel>
        {
            new ProductDetailModel { ProductId = 1, ResourceId = 10 },
            new ProductDetailModel { ProductId = 2, ResourceId = 20 }
        };
        dynamicRepository.Fetch<ProductDetailModel>(Arg.Any<GetProductsDetailCriteria>()).Returns(allProducts);

        // Act
        var xml = service.BuildProductDetailsXML(validProductIds, validResourceIds, productOfferProductDetail);

        // Assert
        Assert.Contains("<ROOT>", xml);
        Assert.Contains("ProductId=\"1\"", xml);
        Assert.DoesNotContain("ProductId=\"2\"", xml);
    }
}
