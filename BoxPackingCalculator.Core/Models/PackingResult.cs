namespace BoxPackingCalculator.Core.Models;

public sealed class PackingResult
{
    public required Product Product { get; init; }

    public int OrderQuantity { get; init; }

    public int FullBoxCount { get; init; }

    public int RemainingQuantity { get; init; }

    public double RemainingOccupancyRate { get; init; }

    public double EstimatedRemainingWeightKg { get; init; }
}