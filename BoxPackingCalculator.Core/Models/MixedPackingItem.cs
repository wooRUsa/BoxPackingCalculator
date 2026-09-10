namespace BoxPackingCalculator.Core.Models;

public sealed class MixedPackingItem
{
    public required Product Product { get; init; }

    public int Quantity { get; init; }

    public double OccupancyRate =>
        Quantity == 0
            ? 0
            : (double)Quantity / Product.CapacityPerBox;

    public double EstimatedWeightKg =>
        Product.FullBoxWeightKg * OccupancyRate;
}