namespace BoxPackingCalculator.Core.Models;

public sealed class ProductPackingInfo
{
    public required Product Product { get; init; }

    public Box? Box { get; init; }


    public bool IsPackingNotApplicable =>
        Product.IsPackingNotApplicable;


    public string ProductName =>
        Product.Name;


    public string BoxDisplaySize =>
        IsPackingNotApplicable
            ? "N/A"
            : Box?.DisplaySize ?? "미지정";


    public int CapacityPerBox =>
        Product.CapacityPerBox;


    public double FullBoxWeightKg =>
        Product.FullBoxWeightKg;
}