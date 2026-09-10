using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Tests;

public class MixedBoxSuggestionTests
{
    [Fact]
    public void Suggestion_G61SixAndG52Four_FillsOneBox()
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

        var suggestion = new MixedBoxSuggestion
        {
            BoxId = "BOX_050_051_053",

            Items = new[]
            {
                new MixedPackingItem
                {
                    Product = g61,
                    Quantity = 6
                },

                new MixedPackingItem
                {
                    Product = g52,
                    Quantity = 4
                }
            }
        };

        // Assert
        Assert.Equal(
            0.6,
            suggestion.Items[0].OccupancyRate,
            3);

        Assert.Equal(
            0.4,
            suggestion.Items[1].OccupancyRate,
            3);

        Assert.Equal(
            1.0,
            suggestion.TotalOccupancyRate,
            3);

        Assert.Equal(
            0.0,
            suggestion.RemainingOccupancyRate,
            3);

        Assert.Equal(
            9.2,
            suggestion.EstimatedWeightKg,
            3);
    }
}