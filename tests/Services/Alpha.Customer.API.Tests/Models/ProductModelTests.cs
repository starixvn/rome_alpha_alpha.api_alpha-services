using Alpha.Customer.API.Models;
using Xunit;

namespace Alpha.Customer.API.Tests.Models;

public class ProductModelTests
{
    [Fact]
    public void ProductModel_CanBeCreated_WithDefaultValues()
    {
        // Arrange & Act
        var product = new ProductModel();

        // Assert
        Assert.Equal(0, product.Id);
        Assert.Null(product.Name);
        Assert.Null(product.Code);
    }

    [Fact]
    public void ProductModel_CanBeCreated_WithSetProperties()
    {
        // Arrange & Act
        var product = new ProductModel
        {
            Id = 101,
            Name = "Test Product",
            Code = "TP001"
        };

        // Assert
        Assert.Equal(101, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("TP001", product.Code);
    }

    [Theory]
    [InlineData(1, "Product1", "P001")]
    [InlineData(999, "Long Product Name With Spaces", "LPNWS")]
    [InlineData(0, "", "")]
    [InlineData(-1, null, null)]
    public void ProductModel_WorksWithVariousValues(int id, string name, string code)
    {
        // Arrange & Act
        var product = new ProductModel
        {
            Id = id,
            Name = name,
            Code = code
        };

        // Assert
        Assert.Equal(id, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(code, product.Code);
    }

    [Fact]
    public void ProductModel_PropertiesCanBeModified()
    {
        // Arrange
        var product = new ProductModel { Id = 1, Name = "Original", Code = "ORIG" };

        // Act
        product.Id = 2;
        product.Name = "Modified";
        product.Code = "MOD";

        // Assert
        Assert.Equal(2, product.Id);
        Assert.Equal("Modified", product.Name);
        Assert.Equal("MOD", product.Code);
    }
}
