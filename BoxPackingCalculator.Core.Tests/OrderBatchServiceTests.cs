using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class OrderBatchServiceTests
{
    [Fact]
    public void Calculate_MultipleOrders_ReturnsAllPackingResults()
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

        var orderBatchService =
            new OrderBatchService(
                orderEntryService);

        var orders = new[]
        {
            (ProductId: "P007", Quantity: 27),
            (ProductId: "P003", Quantity: 16),
            (ProductId: "P004", Quantity: 24)
        };

        // Act
        var results =
            orderBatchService.Calculate(orders);

        // Assert
        Assert.Equal(3, results.Count);

        var her = Assert.Single(
            results,
            x => x.ProductInfo.Product.Id == "P007");

        Assert.Equal(2, her.FullBoxCount);
        Assert.Equal(7, her.RemainingQuantity);


        var g61 = Assert.Single(
            results,
            x => x.ProductInfo.Product.Id == "P003");

        Assert.Equal(1, g61.FullBoxCount);
        Assert.Equal(6, g61.RemainingQuantity);


        var g52 = Assert.Single(
            results,
            x => x.ProductInfo.Product.Id == "P004");

        Assert.Equal(2, g52.FullBoxCount);
        Assert.Equal(4, g52.RemainingQuantity);
    }
}