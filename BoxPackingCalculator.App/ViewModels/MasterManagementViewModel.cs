using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class MasterManagementViewModel : INotifyPropertyChanged
{
    private ProductMasterRowViewModel? _selectedProduct;
    private Box? _selectedBox;


    public ObservableCollection<ProductMasterRowViewModel> Products { get; } = new();

    public ObservableCollection<Box> Boxes { get; } =
        new();


    public ProductMasterRowViewModel? SelectedProduct
    {
        get => _selectedProduct;

        set
        {
            if (_selectedProduct == value)
            {
                return;
            }

            _selectedProduct = value;

            OnPropertyChanged();
        }
    }
    
    public Box? SelectedBox
    {
        get => _selectedBox;

        set
        {
            if (_selectedBox == value)
            {
                return;
            }

            _selectedBox = value;

            OnPropertyChanged();
        }
    }


    public MasterManagementViewModel()
    {
        RefreshProducts();
        RefreshBoxes();
    }


    public void RefreshProducts()
    {
        Products.Clear();

        var products =
            App.Services.ProductMaster.GetAll();

        foreach (var product in products)
        {
            Box? box = null;

            if (!string.IsNullOrWhiteSpace(
                    product.BoxId))
            {
                box =
                    App.Services.BoxCatalog.FindById(
                        product.BoxId);
            }

            Products.Add(
                new ProductMasterRowViewModel
                {
                    Product = product,
                    Box = box
                });
        }
    }


    public void RefreshBoxes()
    {
        Boxes.Clear();

        var boxes =
            App.Services.BoxCatalog.GetAll();

        foreach (var box in boxes)
        {
            Boxes.Add(box);
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