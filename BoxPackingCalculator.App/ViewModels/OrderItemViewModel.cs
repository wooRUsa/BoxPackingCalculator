using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class OrderItemViewModel : INotifyPropertyChanged
{
    private int _quantity = 1;

    public required ProductPackingInfo ProductInfo { get; init; }

    public string ProductId =>
        ProductInfo.Product.Id;

    public string ProductName =>
        ProductInfo.ProductName;

    public string BoxDisplaySize =>
        ProductInfo.BoxDisplaySize;

    public int CapacityPerBox =>
        ProductInfo.CapacityPerBox;

    public double FullBoxWeightKg =>
        ProductInfo.FullBoxWeightKg;


    public int Quantity
    {
        get => _quantity;

        set
        {
            var newQuantity =
                Math.Max(1, value);

            if (_quantity == newQuantity)
            {
                return;
            }

            _quantity = newQuantity;

            OnPropertyChanged();
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}