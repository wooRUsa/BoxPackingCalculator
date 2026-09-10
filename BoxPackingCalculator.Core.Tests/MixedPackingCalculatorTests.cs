using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class MixedPackingCalculatorTests
{
    [Fact]
    public void Calculate_SixG61AndFourG52_CreatesOneFullMixedBox()
    {
        // Arrange
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

        var group = new BoxPackingGroup
        {
            BoxId = "BOX_050_051_053",

            Items = new[]
            {
                new PackingResult
                {
                    Product = g61,
                    OrderQuantity = 16,
                    FullBoxCount = 1,
                    RemainingQuantity = 6,
                    RemainingOccupancyRate = 0.6,
                    EstimatedRemainingWeightKg = 6.0
                },

                new PackingResult
                {
                    Product = g52,
                    OrderQuantity = 24,
                    FullBoxCount = 2,
                    RemainingQuantity = 4,
                    RemainingOccupancyRate = 0.4,
                    EstimatedRemainingWeightKg = 3.2
                }
            }
        };

        var calculator = new MixedPackingCalculator();

        // Act
        var suggestions = calculator.Calculate(group);

        // Assert
        var suggestion = Assert.Single(suggestions);

        Assert.Equal(
            "BOX_050_051_053",
            suggestion.BoxId);

        Assert.Equal(
            1.0,
            suggestion.TotalOccupancyRate,
            3);

        Assert.Equal(
            0.0,
            suggestion.RemainingOccupancyRate,
            3);

        var g61Item = Assert.Single(
            suggestion.Items,
            x => x.Product.Id == "P003");

        Assert.Equal(6, g61Item.Quantity);

        var g52Item = Assert.Single(
            suggestion.Items,
            x => x.Product.Id == "P004");

        Assert.Equal(4, g52Item.Quantity);
    }
    
    [Fact]
public void Calculate_SeventyFivePercentAndFiftyPercent_CreatesTwoBoxes()
{
    // Arrange
    var productA = new Product
    {
        Id = "A",
        Name = "Product A",
        BoxId = "BOX_TEST",
        CapacityPerBox = 8,
        FullBoxWeightKg = 8.0
    };

    var productB = new Product
    {
        Id = "B",
        Name = "Product B",
        BoxId = "BOX_TEST",
        CapacityPerBox = 6,
        FullBoxWeightKg = 6.0
    };

    var group = new BoxPackingGroup
    {
        BoxId = "BOX_TEST",

        Items = new[]
        {
            new PackingResult
            {
                Product = productA,
                OrderQuantity = 6,
                FullBoxCount = 0,
                RemainingQuantity = 6,
                RemainingOccupancyRate = 0.75,
                EstimatedRemainingWeightKg = 6.0
            },

            new PackingResult
            {
                Product = productB,
                OrderQuantity = 3,
                FullBoxCount = 0,
                RemainingQuantity = 3,
                RemainingOccupancyRate = 0.5,
                EstimatedRemainingWeightKg = 3.0
            }
        }
    };

    var calculator = new MixedPackingCalculator();

    // Act
    var suggestions = calculator.Calculate(group);

    // Assert
    Assert.Equal(2, suggestions.Count);

    var orderedSuggestions = suggestions
        .OrderByDescending(x => x.TotalOccupancyRate)
        .ToList();

    var fullBox = orderedSuggestions[0];
    var partialBox = orderedSuggestions[1];

    Assert.Equal(
        1.0,
        fullBox.TotalOccupancyRate,
        3);

    Assert.Equal(
        0.25,
        partialBox.TotalOccupancyRate,
        3);

    var productAInFullBox = Assert.Single(
        fullBox.Items,
        x => x.Product.Id == "A");

    Assert.Equal(4, productAInFullBox.Quantity);

    var productBInFullBox = Assert.Single(
        fullBox.Items,
        x => x.Product.Id == "B");

    Assert.Equal(3, productBInFullBox.Quantity);

    var productAInPartialBox = Assert.Single(
        partialBox.Items,
        x => x.Product.Id == "A");

    Assert.Equal(2, productAInPartialBox.Quantity);
}

    [Fact]
    public void Calculate_ProductWithNoRemainder_IsNotIncludedInSuggestions()
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

        var g52 = new Product
        {
            Id = "P004",
            Name = "G52",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 8.0
        };

        var group = new BoxPackingGroup
        {
            BoxId = "BOX_050_051_053",

            Items = new[]
            {
                new PackingResult
                {
                    Product = her,
                    OrderQuantity = 30,
                    FullBoxCount = 3,
                    RemainingQuantity = 0,
                    RemainingOccupancyRate = 0.0,
                    EstimatedRemainingWeightKg = 0.0
                },

                new PackingResult
                {
                    Product = g52,
                    OrderQuantity = 24,
                    FullBoxCount = 2,
                    RemainingQuantity = 4,
                    RemainingOccupancyRate = 0.4,
                    EstimatedRemainingWeightKg = 3.2
                }
            }
        };

        var calculator = new MixedPackingCalculator();

        // Act
        var suggestions = calculator.Calculate(group);

        // Assert
        var suggestion = Assert.Single(suggestions);

        Assert.Single(suggestion.Items);

        var item = suggestion.Items[0];

        Assert.Equal("P004", item.Product.Id);
        Assert.Equal(4, item.Quantity);

        Assert.DoesNotContain(
            suggestion.Items,
            x => x.Product.Id == "P007");

        Assert.Equal(
            0.4,
            suggestion.TotalOccupancyRate,
            3);
    }
    
    [Fact]
    public void Calculate_WhenAllItemsHaveNoRemainder_ReturnsEmptySuggestions()
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

        var group = new BoxPackingGroup
        {
            BoxId = "BOX_050_051_053",

            Items = new[]
            {
                new PackingResult
                {
                    Product = her,
                    OrderQuantity = 30,
                    FullBoxCount = 3,
                    RemainingQuantity = 0,
                    RemainingOccupancyRate = 0.0,
                    EstimatedRemainingWeightKg = 0.0
                },

                new PackingResult
                {
                    Product = g61,
                    OrderQuantity = 20,
                    FullBoxCount = 2,
                    RemainingQuantity = 0,
                    RemainingOccupancyRate = 0.0,
                    EstimatedRemainingWeightKg = 0.0
                }
            }
        };

        var calculator = new MixedPackingCalculator();

        // Act
        var suggestions = calculator.Calculate(group);

        // Assert
        Assert.Empty(suggestions);
    }
    
    [Fact]
    public void Calculate_WhenGroupContainsDifferentBoxId_ThrowsInvalidOperationException()
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

        var shieldRing = new Product
        {
            Id = "P002",
            Name = "19.83(SHIELD RING)",
            BoxId = "BOX_056_058_057",
            CapacityPerBox = 7,
            FullBoxWeightKg = 15.5
        };

        var group = new BoxPackingGroup
        {
            BoxId = "BOX_050_051_053",

            Items = new[]
            {
                new PackingResult
                {
                    Product = her,
                    OrderQuantity = 27,
                    FullBoxCount = 2,
                    RemainingQuantity = 7,
                    RemainingOccupancyRate = 0.7,
                    EstimatedRemainingWeightKg = 6.3
                },

                new PackingResult
                {
                    Product = shieldRing,
                    OrderQuantity = 8,
                    FullBoxCount = 1,
                    RemainingQuantity = 1,
                    RemainingOccupancyRate = 1.0 / 7.0,
                    EstimatedRemainingWeightKg = 15.5 / 7.0
                }
            }
        };

        var calculator = new MixedPackingCalculator();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => calculator.Calculate(group));
    }
    
    [Fact]
    public void Calculate_WhenProductCapacityPerBoxIsZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidProduct = new Product
        {
            Id = "INVALID",
            Name = "Invalid Product",
            BoxId = "BOX_TEST",
            CapacityPerBox = 0,
            FullBoxWeightKg = 10.0
        };

        var group = new BoxPackingGroup
        {
            BoxId = "BOX_TEST",

            Items = new[]
            {
                new PackingResult
                {
                    Product = invalidProduct,
                    OrderQuantity = 5,
                    FullBoxCount = 0,
                    RemainingQuantity = 5,
                    RemainingOccupancyRate = 0.0,
                    EstimatedRemainingWeightKg = 0.0
                }
            }
        };

        var calculator = new MixedPackingCalculator();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(group));
    }
    
    [Fact]
public void Calculate_PreservesAllRemainingQuantities()
{
    // Arrange
    var productA = new Product
    {
        Id = "A",
        Name = "Product A",
        BoxId = "BOX_TEST",
        CapacityPerBox = 8,
        FullBoxWeightKg = 8.0
    };

    var productB = new Product
    {
        Id = "B",
        Name = "Product B",
        BoxId = "BOX_TEST",
        CapacityPerBox = 6,
        FullBoxWeightKg = 6.0
    };

    var productC = new Product
    {
        Id = "C",
        Name = "Product C",
        BoxId = "BOX_TEST",
        CapacityPerBox = 10,
        FullBoxWeightKg = 10.0
    };

    var group = new BoxPackingGroup
    {
        BoxId = "BOX_TEST",

        Items = new[]
        {
            new PackingResult
            {
                Product = productA,
                OrderQuantity = 6,
                FullBoxCount = 0,
                RemainingQuantity = 6,
                RemainingOccupancyRate = 6.0 / 8.0,
                EstimatedRemainingWeightKg = 6.0
            },

            new PackingResult
            {
                Product = productB,
                OrderQuantity = 3,
                FullBoxCount = 0,
                RemainingQuantity = 3,
                RemainingOccupancyRate = 3.0 / 6.0,
                EstimatedRemainingWeightKg = 3.0
            },

            new PackingResult
            {
                Product = productC,
                OrderQuantity = 4,
                FullBoxCount = 0,
                RemainingQuantity = 4,
                RemainingOccupancyRate = 4.0 / 10.0,
                EstimatedRemainingWeightKg = 4.0
            }
        }
    };

    var calculator = new MixedPackingCalculator();

    // Act
    var suggestions = calculator.Calculate(group);

    // Assert
    var totalA = suggestions
        .SelectMany(x => x.Items)
        .Where(x => x.Product.Id == "A")
        .Sum(x => x.Quantity);

    var totalB = suggestions
        .SelectMany(x => x.Items)
        .Where(x => x.Product.Id == "B")
        .Sum(x => x.Quantity);

    var totalC = suggestions
        .SelectMany(x => x.Items)
        .Where(x => x.Product.Id == "C")
        .Sum(x => x.Quantity);

    Assert.Equal(6, totalA);
    Assert.Equal(3, totalB);
    Assert.Equal(4, totalC);
}

[Fact]
public void Calculate_NoSuggestedBoxExceedsOneHundredPercentOccupancy()
{
    // Arrange
    var productA = new Product
    {
        Id = "A",
        Name = "Product A",
        BoxId = "BOX_TEST",
        CapacityPerBox = 3,
        FullBoxWeightKg = 9.0
    };

    var productB = new Product
    {
        Id = "B",
        Name = "Product B",
        BoxId = "BOX_TEST",
        CapacityPerBox = 4,
        FullBoxWeightKg = 8.0
    };

    var productC = new Product
    {
        Id = "C",
        Name = "Product C",
        BoxId = "BOX_TEST",
        CapacityPerBox = 6,
        FullBoxWeightKg = 6.0
    };

    var group = new BoxPackingGroup
    {
        BoxId = "BOX_TEST",

        Items = new[]
        {
            new PackingResult
            {
                Product = productA,
                OrderQuantity = 2,
                FullBoxCount = 0,
                RemainingQuantity = 2,
                RemainingOccupancyRate = 2.0 / 3.0,
                EstimatedRemainingWeightKg = 6.0
            },

            new PackingResult
            {
                Product = productB,
                OrderQuantity = 3,
                FullBoxCount = 0,
                RemainingQuantity = 3,
                RemainingOccupancyRate = 3.0 / 4.0,
                EstimatedRemainingWeightKg = 6.0
            },

            new PackingResult
            {
                Product = productC,
                OrderQuantity = 5,
                FullBoxCount = 0,
                RemainingQuantity = 5,
                RemainingOccupancyRate = 5.0 / 6.0,
                EstimatedRemainingWeightKg = 5.0
            }
        }
    };

    var calculator = new MixedPackingCalculator();

    // Act
    var suggestions = calculator.Calculate(group);

    // Assert
    Assert.NotEmpty(suggestions);

    Assert.All(
        suggestions,
        suggestion =>
            Assert.True(
                suggestion.TotalOccupancyRate <= 1.0 + 1e-9,
                $"Box occupancy exceeded 100%. " +
                $"Actual: {suggestion.TotalOccupancyRate:P2}"));
}
}