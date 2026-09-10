using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Tests;

public class OrderEntryServiceTests
{
    [Fact]
    public void Calculate_HER27_ReturnsCompleteOrderLineResult()
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

        var productPackingInfoService =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        var packingCalculator =
            new PackingCalculator();

        var orderEntryService =
            new OrderEntryService(
                productPackingInfoService,
                packingCalculator);

        // Act
        var result =
            orderEntryService.Calculate(
                "P007",
                27);

        // Assert
        Assert.Equal(
            "HER(HOT EDGE RING)",
            result.ProductName);

        Assert.Equal(
            "50×51×53",
            result.BoxDisplaySize);

        Assert.Equal(
            10,
            result.CapacityPerBox);

        Assert.Equal(
            9.0,
            result.FullBoxWeightKg);

        Assert.Equal(
            27,
            result.OrderQuantity);

        Assert.Equal(
            2,
            result.FullBoxCount);

        Assert.Equal(
            7,
            result.RemainingQuantity);

        Assert.Equal(
            0.7,
            result.RemainingOccupancyRate,
            3);

        Assert.Equal(
            6.3,
            result.EstimatedRemainingWeightKg,
            3);
    }
    
    [Fact]
    public void Calculate_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
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

        var productPackingInfoService =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        var packingCalculator =
            new PackingCalculator();

        var orderEntryService =
            new OrderEntryService(
                productPackingInfoService,
                packingCalculator);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(
            () => orderEntryService.Calculate(
                "NOT_EXIST_PRODUCT",
                10));
    }
    
    [Fact]
public void Calculate_NotApplicableProduct_PreservesQuantityAndSkipsPackingCalculation()
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

        var productPackingInfoService =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        var packingCalculator =
            new PackingCalculator();

        var orderEntryService =
            new OrderEntryService(
                productPackingInfoService,
                packingCalculator);

        // Act
        var result =
            orderEntryService.Calculate(
                "P900",
                7);

        // Assert
        Assert.Equal(
            "세라믹돔",
            result.ProductName);

        Assert.True(
            result.ProductInfo.Product
                .IsPackingNotApplicable);

        Assert.Null(
            result.ProductInfo.Box);

        Assert.Equal(
            7,
            result.OrderQuantity);

        Assert.Equal(
            0,
            result.FullBoxCount);

        Assert.Equal(
            0,
            result.RemainingQuantity);

        Assert.Equal(
            0,
            result.RemainingOccupancyRate);

        Assert.Equal(
            0,
            result.EstimatedRemainingWeightKg);
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