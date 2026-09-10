using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Tests;

public class PackingPlanServiceTests
{
    [Fact]
    public void Calculate_HER_G61_G52_CreatesOneBoxPlanWithExpectedTotals()
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

        var mixedPackingCalculator =
            new MixedPackingCalculator();

        var packingPlanService =
            new PackingPlanService(
                orderBatchService,
                boxCatalog,
                mixedPackingCalculator);

        var orders = new[]
        {
            (ProductId: "P007", Quantity: 27), // HER
            (ProductId: "P003", Quantity: 16), // G61
            (ProductId: "P004", Quantity: 24)  // G52
        };

        // Act
        var plans =
            packingPlanService.Calculate(orders);

        // Assert
        var plan = Assert.Single(plans);

        Assert.Equal(
            "BOX_050_051_053",
            plan.BoxId);

        Assert.Equal(
            "50×51×53",
            plan.BoxDisplaySize);

        Assert.Equal(
            3,
            plan.OrderLines.Count);

        Assert.Equal(
            5,
            plan.TotalFullBoxCount);

        Assert.Equal(
            2,
            plan.MixedBoxCount);

        Assert.Equal(
            7,
            plan.TotalBoxCount);

        // 각 주문행의 계산 결과도 확인
        var her = Assert.Single(
            plan.OrderLines,
            x => x.ProductInfo.Product.Id == "P007");

        Assert.Equal(2, her.FullBoxCount);
        Assert.Equal(7, her.RemainingQuantity);

        var g61 = Assert.Single(
            plan.OrderLines,
            x => x.ProductInfo.Product.Id == "P003");

        Assert.Equal(1, g61.FullBoxCount);
        Assert.Equal(6, g61.RemainingQuantity);

        var g52 = Assert.Single(
            plan.OrderLines,
            x => x.ProductInfo.Product.Id == "P004");

        Assert.Equal(2, g52.FullBoxCount);
        Assert.Equal(4, g52.RemainingQuantity);

        // 혼합 추천 박스가 100%를 넘지 않는지 확인
        Assert.All(
            plan.MixedSuggestions,
            suggestion =>
                Assert.True(
                    suggestion.TotalOccupancyRate <= 1.0 + 1e-9));
    }
    
    [Fact]
public void Calculate_OrdersWithDifferentBoxes_CreatesSeparatePlans()
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

    var mixedPackingCalculator =
        new MixedPackingCalculator();

    var packingPlanService =
        new PackingPlanService(
            orderBatchService,
            boxCatalog,
            mixedPackingCalculator);

    var orders = new[]
    {
        (ProductId: "P007", Quantity: 27), // HER - 50×51×53
        (ProductId: "P003", Quantity: 16), // G61 - 50×51×53
        (ProductId: "P002", Quantity: 8)   // 19.83 - 56×58×57
    };

    // Act
    var plans =
        packingPlanService.Calculate(orders);

    // Assert
    Assert.Equal(
        2,
        plans.Count);

    var box505153 = Assert.Single(
        plans,
        plan => plan.BoxId == "BOX_050_051_053");

    Assert.Equal(
        "50×51×53",
        box505153.BoxDisplaySize);

    Assert.Equal(
        2,
        box505153.OrderLines.Count);

    Assert.Contains(
        box505153.OrderLines,
        line => line.ProductInfo.Product.Id == "P007");

    Assert.Contains(
        box505153.OrderLines,
        line => line.ProductInfo.Product.Id == "P003");


    var box565857 = Assert.Single(
        plans,
        plan => plan.BoxId == "BOX_056_058_057");

    Assert.Equal(
        "56×58×57",
        box565857.BoxDisplaySize);

    var shieldRing = Assert.Single(
        box565857.OrderLines);

    Assert.Equal(
        "P002",
        shieldRing.ProductInfo.Product.Id);

    Assert.Equal(
        1,
        shieldRing.FullBoxCount);

    Assert.Equal(
        1,
        shieldRing.RemainingQuantity);
}

    [Fact]
public void Calculate_MultipleNotApplicableProducts_CreatesSingleNotApplicablePlan()
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
                    Name = "세라믹돔 A",
                    BoxId = Product.NotApplicableBoxId,
                    CapacityPerBox = 0,
                    FullBoxWeightKg = 0
                },
                new Product
                {
                    Id = "P901",
                    Name = "세라믹돔 B",
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

        var orderBatchService =
            new OrderBatchService(
                orderEntryService);

        var mixedPackingCalculator =
            new MixedPackingCalculator();

        var packingPlanService =
            new PackingPlanService(
                orderBatchService,
                boxCatalog,
                mixedPackingCalculator);

        var orders = new[]
        {
            (ProductId: "P900", Quantity: 3),
            (ProductId: "P901", Quantity: 5)
        };

        // Act
        var plans =
            packingPlanService.Calculate(
                orders);

        // Assert
        var plan =
            Assert.Single(
                plans);

        Assert.True(
            plan.IsPackingNotApplicable);

        Assert.Null(
            plan.Box);

        Assert.Equal(
            Product.NotApplicableBoxId,
            plan.BoxId);

        Assert.Equal(
            "N/A",
            plan.BoxDisplaySize);

        Assert.Equal(
            2,
            plan.OrderLines.Count);

        Assert.Empty(
            plan.MixedSuggestions);

        Assert.Equal(
            0,
            plan.TotalFullBoxCount);

        Assert.Equal(
            0,
            plan.MixedBoxCount);

        Assert.Equal(
            0,
            plan.TotalBoxCount);

        var productA =
            Assert.Single(
                plan.OrderLines,
                line =>
                    line.ProductInfo.Product.Id
                    == "P900");

        Assert.Equal(
            3,
            productA.OrderQuantity);

        var productB =
            Assert.Single(
                plan.OrderLines,
                line =>
                    line.ProductInfo.Product.Id
                    == "P901");

        Assert.Equal(
            5,
            productB.OrderQuantity);
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