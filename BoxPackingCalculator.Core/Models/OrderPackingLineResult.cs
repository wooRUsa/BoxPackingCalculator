namespace BoxPackingCalculator.Core.Models;

public sealed class OrderPackingLineResult
{
    public required ProductPackingInfo ProductInfo { get; init; }

    public required PackingResult PackingResult { get; init; }

    public string ProductName =>
        ProductInfo.ProductName;

    public string BoxDisplaySize =>
        ProductInfo.BoxDisplaySize;

    public int CapacityPerBox =>
        ProductInfo.CapacityPerBox;

    public double FullBoxWeightKg =>
        ProductInfo.FullBoxWeightKg;

    public int OrderQuantity =>
        PackingResult.OrderQuantity;

    public int FullBoxCount =>
        PackingResult.FullBoxCount;

    public int RemainingQuantity =>
        PackingResult.RemainingQuantity;

    public double RemainingOccupancyRate =>
        PackingResult.RemainingOccupancyRate;

    public double EstimatedRemainingWeightKg =>
        PackingResult.EstimatedRemainingWeightKg;
}