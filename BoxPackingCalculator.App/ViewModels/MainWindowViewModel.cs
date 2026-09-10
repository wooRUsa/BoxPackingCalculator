using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BoxPackingCalculator.App.Commands;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private string _searchKeyword = string.Empty;
    private ProductPackingInfo? _selectedSearchResult;
    private OrderItemViewModel? _selectedOrderItem;
    private string _packingStatus =
        "주문을 입력한 뒤 포장 계산을 실행해주세요.";
    private string _shippingDestination =
        string.Empty;

    private string _invoiceNumber =
        string.Empty;

    public MainWindowViewModel()
    {
        AddOrderCommand =
            new RelayCommand(
                _ => AddSelectedProduct(),
                _ => SelectedSearchResult is not null);

        RemoveOrderCommand =
            new RelayCommand(
                _ => RemoveSelectedOrder(),
                _ => SelectedOrderItem is not null);

        CalculatePackingCommand =
            new RelayCommand(
                _ => CalculatePackingPlan(),
                _ => OrderItems.Count > 0);
        
        ResetCommand =
            new RelayCommand(
                _ => ConfirmAndReset());
    }


    public string SearchKeyword
    {
        get => _searchKeyword;

        set
        {
            if (_searchKeyword == value)
            {
                return;
            }

            _searchKeyword = value;

            OnPropertyChanged();

            SearchProducts();
        }
    }
    
    public string PackingStatus
    {
        get => _packingStatus;

        private set
        {
            if (_packingStatus == value)
            {
                return;
            }

            _packingStatus = value;

            OnPropertyChanged();
        }
    }
    
    public string ShippingDestination
    {
        get => _shippingDestination;

        set
        {
            if (_shippingDestination == value)
            {
                return;
            }

            _shippingDestination = value;

            OnPropertyChanged();
        }
    }


    public string InvoiceNumber
    {
        get => _invoiceNumber;

        set
        {
            if (_invoiceNumber == value)
            {
                return;
            }

            _invoiceNumber = value;

            OnPropertyChanged();
        }
    }


    public ProductPackingInfo? SelectedSearchResult
    {
        get => _selectedSearchResult;

        set
        {
            if (_selectedSearchResult == value)
            {
                return;
            }

            _selectedSearchResult = value;

            OnPropertyChanged();

            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public OrderItemViewModel? SelectedOrderItem
    {
        get => _selectedOrderItem;

        set
        {
            if (_selectedOrderItem == value)
            {
                return;
            }

            _selectedOrderItem = value;

            OnPropertyChanged();

            CommandManager.InvalidateRequerySuggested();
        }
    }


    public ObservableCollection<ProductPackingInfo> SearchResults { get; } =
        new();

    public ObservableCollection<OrderItemViewModel> OrderItems { get; } =
        new();
    
    public ObservableCollection<BoxPackingPlanResult> PackingPlans { get; } =
        new();
    
    public ObservableCollection<MixedBoxResultViewModel> MixedBoxResults { get; } =
        new();
    
    public ObservableCollection<OrderLineResultViewModel> OrderLineResults { get; } =
        new();
    
    public ObservableCollection<BoxResultGroupViewModel> BoxResultGroups { get; } = new();
    
    public int OverallFullBoxCount =>
        BoxResultGroups.Sum(
            group => group.TotalFullBoxCount);


    public double OverallFullBoxWeightKg =>
        BoxResultGroups.Sum(
            group => group.TotalFullBoxWeightKg);


    public int OverallMixedBoxCount =>
        BoxResultGroups.Sum(
            group => group.MixedBoxCount);


    public ICommand AddOrderCommand { get; }
    public ICommand RemoveOrderCommand { get; }
    public ICommand CalculatePackingCommand { get; }
    
    public ICommand ResetCommand { get; }


    private void SearchProducts()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchKeyword))
        {
            return;
        }

        var results =
            App.Services.ProductPackingInfo.Search(
                SearchKeyword);

        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
    }


    private void AddSelectedProduct()
    {
        if (SelectedSearchResult is null)
        {
            return;
        }

        var alreadyExists =
            OrderItems.Any(
                item =>
                    item.ProductId ==
                    SelectedSearchResult.Product.Id);

        if (alreadyExists)
        {
            return;
        }

        var orderItem =
            new OrderItemViewModel
            {
                ProductInfo = SelectedSearchResult,
                Quantity = 1
            };

        orderItem.PropertyChanged +=
            OnOrderItemPropertyChanged;

        OrderItems.Add(orderItem);

        ClearPackingResults();

        SearchKeyword = string.Empty;
        SelectedSearchResult = null;
    }
    
    private void RemoveSelectedOrder()
    {
        if (SelectedOrderItem is null)
        {
            return;
        }

        SelectedOrderItem.PropertyChanged -=
            OnOrderItemPropertyChanged;

        OrderItems.Remove(
            SelectedOrderItem);

        SelectedOrderItem = null;

        ClearPackingResults();
    }
    
    private void CalculatePackingPlan()
    {
        PackingPlans.Clear();
        MixedBoxResults.Clear();
        OrderLineResults.Clear();
        BoxResultGroups.Clear();

        var orders =
            OrderItems
                .Select(item =>
                (
                    ProductId: item.ProductId,
                    Quantity: item.Quantity
                ))
                .ToList();

        var plans =
            App.Services.PackingPlan.Calculate(
                orders);

        foreach (var plan in plans)
        {
            PackingPlans.Add(plan);

            BoxResultGroups.Add(
                new BoxResultGroupViewModel(plan));

            foreach (var orderLine in plan.OrderLines)
            {
                OrderLineResults.Add(
                    new OrderLineResultViewModel
                    {
                        Result = orderLine
                    });
            }

            for (var i = 0; i < plan.MixedSuggestions.Count; i++)
            {
                var suggestion =
                    plan.MixedSuggestions[i];

                MixedBoxResults.Add(
                    new MixedBoxResultViewModel
                    {
                        BoxNumber = i + 1,
                        BoxDisplaySize = plan.BoxDisplaySize,
                        Suggestion = suggestion
                    });
            }
        }
        
        OnPropertyChanged(
            nameof(OverallFullBoxCount));

        OnPropertyChanged(
            nameof(OverallFullBoxWeightKg));

        OnPropertyChanged(
            nameof(OverallMixedBoxCount));
        
        PackingStatus =
            "포장 계산이 완료되었습니다.";
    }
    
    private void ClearPackingResults()
    {
        PackingPlans.Clear();
        OrderLineResults.Clear();
        MixedBoxResults.Clear();
        BoxResultGroups.Clear();

        OnPropertyChanged(
            nameof(OverallFullBoxCount));

        OnPropertyChanged(
            nameof(OverallFullBoxWeightKg));

        OnPropertyChanged(
            nameof(OverallMixedBoxCount));
        
        PackingStatus =
            "주문 내용이 변경되었습니다. 포장 계산을 다시 실행해주세요.";
    }
    
    private void ConfirmAndReset()
    {
        var confirmed =
            App.Services.Confirmation.ConfirmReset();

        if (!confirmed)
        {
            return;
        }

        ResetAll();
    }
    
    private void ResetAll()
    {
        foreach (var orderItem in OrderItems)
        {
            orderItem.PropertyChanged -=
                OnOrderItemPropertyChanged;
        }

        OrderItems.Clear();

        SelectedOrderItem = null;

        SearchKeyword = string.Empty;
        SelectedSearchResult = null;
        SearchResults.Clear();
        
        ShippingDestination =
            string.Empty;

        InvoiceNumber =
            string.Empty;
        PackingPlans.Clear();
        OrderLineResults.Clear();
        MixedBoxResults.Clear();
        BoxResultGroups.Clear();
        
        OnPropertyChanged(
            nameof(OverallFullBoxCount));

        OnPropertyChanged(
            nameof(OverallFullBoxWeightKg));

        OnPropertyChanged(
            nameof(OverallMixedBoxCount));

        PackingStatus =
            "주문을 입력한 뒤 포장 계산을 실행해주세요.";

        CommandManager.InvalidateRequerySuggested();
    }
    
    private void OnOrderItemPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName ==
            nameof(OrderItemViewModel.Quantity))
        {
            ClearPackingResults();
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