using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class ProductMasterRowViewModel
{
    public required Product Product
    {
        get;
        init;
    }


    public Box? Box
    {
        get;
        init;
    }


    public string ProductId =>
        Product.Id;


    public string ProductName =>
        Product.Name;


    public string BoxDisplaySize =>
        Product.IsPackingNotApplicable
            ? "N/A"
            : Box?.DisplaySize
              ?? "미지정";


    public string CapacityPerBoxDisplay =>
        Product.IsPackingNotApplicable
            ? string.Empty
            : Product.CapacityPerBox > 0
                ? Product.CapacityPerBox.ToString()
                : "미입력";


    public string FullBoxWeightKgDisplay =>
        Product.IsPackingNotApplicable
            ? string.Empty
            : Product.FullBoxWeightKg > 0
                ? $"{Product.FullBoxWeightKg:0.##} kg"
                : "미입력";


    public bool IsPackingInfoComplete =>
        Product.IsPackingInfoComplete;


    public string PackingInfoStatus =>
        Product.PackingInfoStatus;
}