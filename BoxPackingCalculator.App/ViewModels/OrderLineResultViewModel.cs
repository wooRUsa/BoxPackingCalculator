using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class OrderLineResultViewModel
{
    public required OrderPackingLineResult Result { get; init; }


    public string ProductName =>
        Result.ProductName;

    public string BoxDisplaySize =>
        Result.BoxDisplaySize;

    public int OrderQuantity =>
        Result.OrderQuantity;

    public int FullBoxCount =>
        Result.FullBoxCount;
    
    public double FullBoxWeightKg =>
        Result.FullBoxWeightKg;
    
    public double FullBoxTotalWeightKg =>
        FullBoxCount * FullBoxWeightKg;

    public int RemainingQuantity =>
        Result.RemainingQuantity;

    public double RemainingOccupancyPercent =>
        Result.RemainingOccupancyRate * 100;

    public double EstimatedRemainingWeightKg =>
        Result.EstimatedRemainingWeightKg;
    
    public bool IsPackingNotApplicable =>
        Result.ProductInfo.Product.IsPackingNotApplicable;


    public int? CapacityPerBoxDisplay =>
        IsPackingNotApplicable
            ? null
            : Result.CapacityPerBox;


    public int? FullBoxCountDisplay =>
        IsPackingNotApplicable
            ? null
            : Result.FullBoxCount;


    public string FullBoxTotalWeightDisplay =>
        IsPackingNotApplicable
            ? string.Empty
            : $"{FullBoxTotalWeightKg:0.##} kg";


    public int? RemainingQuantityDisplay =>
        IsPackingNotApplicable
            ? null
            : Result.RemainingQuantity;
}