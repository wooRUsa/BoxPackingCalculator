using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class OrderPackingCalculatorTests
{
    [Fact]
    public void Calculate_GroupsOrdersByBoxId()
    {
        // Arrange
        var her = new Product
        {
            Id = "P007",
            Name = "HER(HOT EDGE RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 9.0
        };

        var g61 = new Product
        {
            Id = "P003",
            Name = "G61",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 10.0
        };

        var shieldRing = new Product
        {
            Id = "P002",
            Name = "19.83(SHIELD RING)",
            BoxId = "BOX_056_058_057",
            CapacityPerBox = 7,
            FullBoxWeightKg = 15.5
        };

        var g52 = new Product
        {
            Id = "P004",
            Name = "G52",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 8.0
        };

        var orders = new[]
        {
            new OrderItem
            {
                Product = her,
                Quantity = 27
            },
            new OrderItem
            {
                Product = g61,
                Quantity = 16
            },
            new OrderItem
            {
                Product = shieldRing,
                Quantity = 8
            },
            new OrderItem
            {
                Product = g52,
                Quantity = 24
            }
        };

        var packingCalculator = new PackingCalculator();

        var orderPackingCalculator =
            new OrderPackingCalculator(packingCalculator);

        // Act
        var groups = orderPackingCalculator.Calculate(orders);

        // Assert
        Assert.Equal(2, groups.Count);

        var box505153 =
            Assert.Single(
                groups,
                group => group.BoxId == "BOX_050_051_053");

        Assert.Equal(3, box505153.Items.Count);

        Assert.Contains(
            box505153.Items,
            item => item.Product.Id == "P007");

        Assert.Contains(
            box505153.Items,
            item => item.Product.Id == "P003");

        Assert.Contains(
            box505153.Items,
            item => item.Product.Id == "P004");


        var box565857 =
            Assert.Single(
                groups,
                group => group.BoxId == "BOX_056_058_057");

        Assert.Single(box565857.Items);

        Assert.Equal(
            "P002",
            box565857.Items[0].Product.Id);
    }
    
    [Fact]
public void Calculate_GroupedItemsContainCorrectPackingResults()
{
    // Arrange
    var her = new Product
    {
        Id = "P007",
        Name = "HER(HOT EDGE RING)",
        BoxId = "BOX_050_051_053",
        CapacityPerBox = 10,
        FullBoxWeightKg = 9.0
    };

    var g61 = new Product
    {
        Id = "P003",
        Name = "G61",
        BoxId = "BOX_050_051_053",
        CapacityPerBox = 10,
        FullBoxWeightKg = 10.0
    };

    var g52 = new Product
    {
        Id = "P004",
        Name = "G52",
        BoxId = "BOX_050_051_053",
        CapacityPerBox = 10,
        FullBoxWeightKg = 8.0
    };

    var orders = new[]
    {
        new OrderItem
        {
            Product = her,
            Quantity = 27
        },
        new OrderItem
        {
            Product = g61,
            Quantity = 16
        },
        new OrderItem
        {
            Product = g52,
            Quantity = 24
        }
    };

    var packingCalculator = new PackingCalculator();

    var orderPackingCalculator =
        new OrderPackingCalculator(packingCalculator);

    // Act
    var groups = orderPackingCalculator.Calculate(orders);

    // Assert
    var group = Assert.Single(
        groups,
        x => x.BoxId == "BOX_050_051_053");

    var herResult = Assert.Single(
        group.Items,
        x => x.Product.Id == "P007");

    Assert.Equal(2, herResult.FullBoxCount);
    Assert.Equal(7, herResult.RemainingQuantity);
    Assert.Equal(0.7, herResult.RemainingOccupancyRate, 3);


    var g61Result = Assert.Single(
        group.Items,
        x => x.Product.Id == "P003");

    Assert.Equal(1, g61Result.FullBoxCount);
    Assert.Equal(6, g61Result.RemainingQuantity);
    Assert.Equal(0.6, g61Result.RemainingOccupancyRate, 3);


    var g52Result = Assert.Single(
        group.Items,
        x => x.Product.Id == "P004");

    Assert.Equal(2, g52Result.FullBoxCount);
    Assert.Equal(4, g52Result.RemainingQuantity);
    Assert.Equal(0.4, g52Result.RemainingOccupancyRate, 3);
}
}