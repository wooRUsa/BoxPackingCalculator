using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Tests;

public class ProductPackingInfoServiceTests
{
    [Fact]
    public void FindByProductId_HER_ReturnsProductAndBoxInformation()
    {
        // Arrange
        var productFilePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var boxFilePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "boxes.json"));

        var productRepository =
            new ProductRepository(productFilePath);

        var boxRepository =
            new BoxRepository(boxFilePath);

        var productCatalog =
            new ProductCatalogService(productRepository);

        var boxCatalog =
            new BoxCatalogService(boxRepository);

        var service =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        // Act
        var info =
            service.FindByProductId("P007");

        // Assert
        Assert.NotNull(info);

        Assert.Equal(
            "HER(HOT EDGE RING)",
            info.ProductName);

        Assert.Equal(
            "50×51×53",
            info.BoxDisplaySize);

        Assert.Equal(
            10,
            info.CapacityPerBox);

        Assert.Equal(
            9.0,
            info.FullBoxWeightKg);

        Assert.Equal(
            "P007",
            info.Product.Id);

        var box =
            Assert.IsType<Box>(
                info.Box);

        Assert.Equal(
            "BOX_050_051_053",
            box.Id);
    }
    
    [Fact]
    public void Search_HER_ReturnsPackingInformation()
    {
        // Arrange
        var productFilePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var boxFilePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "boxes.json"));

        var productRepository =
            new ProductRepository(productFilePath);

        var boxRepository =
            new BoxRepository(boxFilePath);

        var productCatalog =
            new ProductCatalogService(productRepository);

        var boxCatalog =
            new BoxCatalogService(boxRepository);

        var service =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        // Act
        var results =
            service.Search("HER");

        // Assert
        var info = Assert.Single(
            results,
            x => x.Product.Id == "P007");

        Assert.Equal(
            "HER(HOT EDGE RING)",
            info.ProductName);

        Assert.Equal(
            "50×51×53",
            info.BoxDisplaySize);

        Assert.Equal(
            10,
            info.CapacityPerBox);

        Assert.Equal(
            9.0,
            info.FullBoxWeightKg);
    }
    [Fact]
public void Search_NotApplicableProduct_ReturnsWithoutPhysicalBox()
{
    // Arrange
    var tempDirectory =
        Path.Combine(
            Path.GetTempPath(),
            "BoxPackingCalculatorTests",
            Guid.NewGuid().ToString("N"));

    Directory.CreateDirectory(
        tempDirectory);

    var productFilePath =
        Path.Combine(
            tempDirectory,
            "products.json");

    var boxFilePath =
        Path.Combine(
            tempDirectory,
            "boxes.json");

    try
    {
        var productRepository =
            new ProductRepository(
                productFilePath);

        productRepository.Save(
            new[]
            {
                new Product
                {
                    Id = "P900",
                    Name = "세라믹돔",
                    BoxId = Product.NotApplicableBoxId,
                    CapacityPerBox = 0,
                    FullBoxWeightKg = 0
                }
            });

        File.WriteAllText(
            boxFilePath,
            "[]");

        var boxRepository =
            new BoxRepository(
                boxFilePath);

        var productCatalog =
            new ProductCatalogService(
                productRepository);

        var boxCatalog =
            new BoxCatalogService(
                boxRepository);

        var service =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        // Act
        var results =
            service.Search(
                "세라믹");

        // Assert
        var info =
            Assert.Single(results);

        Assert.Equal(
            "P900",
            info.Product.Id);

        Assert.Equal(
            "세라믹돔",
            info.ProductName);

        Assert.True(
            info.Product.IsPackingNotApplicable);

        Assert.True(
            info.Product.IsAvailableForOrdering);

        Assert.Equal(
            "N/A",
            info.BoxDisplaySize);

        Assert.Null(
            info.Box);

        Assert.Equal(
            0,
            info.CapacityPerBox);

        Assert.Equal(
            0,
            info.FullBoxWeightKg);
    }
    finally
    {
        if (Directory.Exists(
                tempDirectory))
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }
}
    
}