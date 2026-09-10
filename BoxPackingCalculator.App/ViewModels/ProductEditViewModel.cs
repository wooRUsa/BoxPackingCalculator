using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class ProductEditViewModel : INotifyPropertyChanged
{
    private string _productName = string.Empty;
    private ProductBoxOption? _selectedBoxOption;
    private string _capacityPerBoxText = string.Empty;
    private string _fullBoxWeightKgText = string.Empty;
    private readonly string? _editingProductId;


    public ObservableCollection<ProductBoxOption> BoxOptions { get; } =
        new();


    public string ProductName
    {
        get => _productName;

        set
        {
            if (_productName == value)
            {
                return;
            }

            _productName = value;

            OnPropertyChanged();
        }
    }


    public ProductBoxOption? SelectedBoxOption
    {
        get => _selectedBoxOption;

        set
        {
            if (_selectedBoxOption == value)
            {
                return;
            }

            _selectedBoxOption = value;

            if (IsPackingNotApplicable)
            {
                CapacityPerBoxText = string.Empty;
                FullBoxWeightKgText = string.Empty;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPackingNotApplicable));
            OnPropertyChanged(nameof(IsPackingFieldsEnabled));
        }
    }


    public bool IsPackingNotApplicable =>
        SelectedBoxOption?.IsNotApplicable == true;


    public bool IsPackingFieldsEnabled =>
        !IsPackingNotApplicable;


    public string CapacityPerBoxText
    {
        get => _capacityPerBoxText;

        set
        {
            if (_capacityPerBoxText == value)
            {
                return;
            }

            _capacityPerBoxText = value;

            OnPropertyChanged();
        }
    }


    public string FullBoxWeightKgText
    {
        get => _fullBoxWeightKgText;

        set
        {
            if (_fullBoxWeightKgText == value)
            {
                return;
            }

            _fullBoxWeightKgText = value;

            OnPropertyChanged();
        }
    }


    public ProductEditViewModel()
    {
        LoadBoxes();
    }


    public ProductEditViewModel(
        ProductMasterRowViewModel productRow)
    {
        var product =
            productRow.Product;

        _editingProductId =
            product.Id;

        LoadBoxes();


        ProductName =
            product.Name;


        SelectedBoxOption =
            BoxOptions.FirstOrDefault(
                option =>
                    string.Equals(
                        option.BoxId,
                        product.BoxId,
                        StringComparison.OrdinalIgnoreCase));


        CapacityPerBoxText =
            product.CapacityPerBox > 0
                ? product.CapacityPerBox.ToString()
                : string.Empty;


        FullBoxWeightKgText =
            product.FullBoxWeightKg > 0
                ? product.FullBoxWeightKg.ToString()
                : string.Empty;
    }


    private void LoadBoxes()
    {
        BoxOptions.Clear();

        BoxOptions.Add(
            new ProductBoxOption
            {
                BoxId = Product.NotApplicableBoxId,
                DisplayText = "N/A"
            });

        var boxes =
            App.Services.BoxCatalog.GetAll();

        foreach (var box in boxes)
        {
            BoxOptions.Add(
                new ProductBoxOption
                {
                    BoxId = box.Id,
                    DisplayText = box.DisplaySize
                });
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
    
    public void SaveNewProduct()
    {
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            throw new InvalidOperationException(
                "제품명을 입력해주세요.");
        }


        var capacityPerBox = 0;

        if (!string.IsNullOrWhiteSpace(
                CapacityPerBoxText)
            && !int.TryParse(
                CapacityPerBoxText,
                out capacityPerBox))
        {
            throw new InvalidOperationException(
                "입수량은 정수로 입력해주세요.");
        }


        var fullBoxWeightKg = 0.0;

        if (!string.IsNullOrWhiteSpace(
                FullBoxWeightKgText)
            && !double.TryParse(
                FullBoxWeightKgText,
                out fullBoxWeightKg))
        {
            throw new InvalidOperationException(
                "완박스 무게는 숫자로 입력해주세요.");
        }
        
        if (IsPackingNotApplicable)
        {
            capacityPerBox = 0;
            fullBoxWeightKg = 0;
        }

        App.Services.ProductMaster.Add(
            ProductName,
            SelectedBoxOption?.BoxId ?? string.Empty,
            capacityPerBox,
            fullBoxWeightKg);


        App.Services.ProductCatalog.Reload();
    }
    
    public void SaveChanges()
    {
        if (_editingProductId is null)
        {
            SaveNewProduct();
            return;
        }


        if (string.IsNullOrWhiteSpace(ProductName))
        {
            throw new InvalidOperationException(
                "제품명을 입력해주세요.");
        }


        var capacityPerBox = 0;

        if (!string.IsNullOrWhiteSpace(
                CapacityPerBoxText)
            && !int.TryParse(
                CapacityPerBoxText,
                out capacityPerBox))
        {
            throw new InvalidOperationException(
                "입수량은 정수로 입력해주세요.");
        }


        var fullBoxWeightKg = 0.0;

        if (!string.IsNullOrWhiteSpace(
                FullBoxWeightKgText)
            && !double.TryParse(
                FullBoxWeightKgText,
                out fullBoxWeightKg))
        {
            throw new InvalidOperationException(
                "완박스 무게는 숫자로 입력해주세요.");
        }
        
        if (IsPackingNotApplicable)
        {
            capacityPerBox = 0;
            fullBoxWeightKg = 0;
        }


        App.Services.ProductMaster.Update(
            _editingProductId,
            ProductName,
            SelectedBoxOption?.BoxId ?? string.Empty,
            capacityPerBox,
            fullBoxWeightKg);


        App.Services.ProductCatalog.Reload();
    }
}