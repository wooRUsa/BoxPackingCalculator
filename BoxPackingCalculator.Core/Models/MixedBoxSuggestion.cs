namespace BoxPackingCalculator.Core.Models;

public sealed class MixedBoxSuggestion
{
    public required string BoxId { get; init; }

    public required IReadOnlyList<MixedPackingItem> Items { get; init; }

    public double TotalOccupancyRate =>
        Items.Sum(item => item.OccupancyRate);

    public double RemainingOccupancyRate =>
        Math.Max(0, 1.0 - TotalOccupancyRate);

    public double EstimatedWeightKg =>
        Items.Sum(item => item.EstimatedWeightKg);
}