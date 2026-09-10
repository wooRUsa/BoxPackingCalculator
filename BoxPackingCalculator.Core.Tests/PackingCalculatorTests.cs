using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class PackingCalculatorTests
{
    [Fact]
    public void Calculate_HER_27_ReturnsExpectedResult()
    {
        // Arrange
        var product = new Product
        {
            Id = "P007",
            Name = "HER(HOT EDGE RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 9.0
        };

        var order = new OrderItem
        {
            Product = product,
            Quantity = 27
        };

        var calculator = new PackingCalculator();

        // Act
        var result = calculator.Calculate(order);

        // Assert
        Assert.Equal(27, result.OrderQuantity);
        Assert.Equal(2, result.FullBoxCount);
        Assert.Equal(7, result.RemainingQuantity);
        Assert.Equal(0.7, result.RemainingOccupancyRate, 3);
        Assert.Equal(6.3, result.EstimatedRemainingWeightKg, 3);
    }
    
    [Fact]
    public void Calculate_HER_30_ReturnsThreeFullBoxesWithNoRemainder()
    {
        // Arrange
        var product = new Product
        {
            Id = "P007",
            Name = "HER(HOT EDGE RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 9.0
        };

        var order = new OrderItem
        {
            Product = product,
            Quantity = 30
        };

        var calculator = new PackingCalculator();

        // Act
        var result = calculator.Calculate(order);

        // Assert
        Assert.Equal(30, result.OrderQuantity);
        Assert.Equal(3, result.FullBoxCount);
        Assert.Equal(0, result.RemainingQuantity);
        Assert.Equal(0, result.RemainingOccupancyRate);
        Assert.Equal(0, result.EstimatedRemainingWeightKg);
    }
    
    [Fact]
    public void Calculate_CoverRing_20_ReturnsExpectedResult()
    {
        // Arrange
        var product = new Product
        {
            Id = "P001",
            Name = "31.3(COVER RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 6,
            FullBoxWeightKg = 13.0
        };

        var order = new OrderItem
        {
            Product = product,
            Quantity = 20
        };

        var calculator = new PackingCalculator();

        // Act
        var result = calculator.Calculate(order);

        // Assert
        Assert.Equal(20, result.OrderQuantity);
        Assert.Equal(3, result.FullBoxCount);
        Assert.Equal(2, result.RemainingQuantity);

        Assert.Equal(
            2.0 / 6.0,
            result.RemainingOccupancyRate,
            3);

        Assert.Equal(
            13.0 * (2.0 / 6.0),
            result.EstimatedRemainingWeightKg,
            3);
    }
    
    [Fact]
    public void Calculate_WhenCapacityPerBoxIsZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var product = new Product
        {
            Id = "TEST001",
            Name = "INVALID PRODUCT",
            BoxId = "BOX_TEST",
            CapacityPerBox = 0,
            FullBoxWeightKg = 10.0
        };

        var order = new OrderItem
        {
            Product = product,
            Quantity = 10
        };

        var calculator = new PackingCalculator();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(order));
    }
    
    [Fact]
    public void Calculate_WhenOrderQuantityIsNegative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var product = new Product
        {
            Id = "P007",
            Name = "HER(HOT EDGE RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 9.0
        };

        var order = new OrderItem
        {
            Product = product,
            Quantity = -1
        };

        var calculator = new PackingCalculator();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(order));
    }
    
    [Fact]
    public void Calculate_WhenOrderQuantityIsZero_ReturnsZeroResult()
    {
        // Arrange
        var product = new Product
        {
            Id = "P007",
            Name = "HER(HOT EDGE RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 9.0
        };

        var order = new OrderItem
        {
            Product = product,
            Quantity = 0
        };

        var calculator = new PackingCalculator();

        // Act
        var result = calculator.Calculate(order);

        // Assert
        Assert.Equal(0, result.OrderQuantity);
        Assert.Equal(0, result.FullBoxCount);
        Assert.Equal(0, result.RemainingQuantity);
        Assert.Equal(0, result.RemainingOccupancyRate);
        Assert.Equal(0, result.EstimatedRemainingWeightKg);
    }
}