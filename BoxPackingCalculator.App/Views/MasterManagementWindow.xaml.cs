using System.Windows;
using BoxPackingCalculator.App.ViewModels;

namespace BoxPackingCalculator.App.Views;

public partial class MasterManagementWindow : Window
{
    public MasterManagementWindow()
    {
        InitializeComponent();

        DataContext =
            new MasterManagementViewModel();
    }


    private void AddProductButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new ProductEditWindow
            {
                Owner = this
            };

        var result =
            window.ShowDialog();

        if (result != true)
        {
            return;
        }

        if (DataContext is not MasterManagementViewModel viewModel)
        {
            return;
        }

        viewModel.RefreshProducts();
    }
    
    private void EditProductButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not MasterManagementViewModel viewModel)
        {
            return;
        }

        if (viewModel.SelectedProduct is null)
        {
            MessageBox.Show(
                "수정할 제품을 먼저 선택해주세요.",
                "제품 선택",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window =
            new ProductEditWindow(
                viewModel.SelectedProduct)
            {
                Owner = this
            };

        var result =
            window.ShowDialog();

        if (result == true)
        {
            viewModel.RefreshProducts();
        }
    }
    
    private void DeleteProductButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not MasterManagementViewModel viewModel)
        {
            return;
        }

        if (viewModel.SelectedProduct is null)
        {
            MessageBox.Show(
                "삭제할 제품을 먼저 선택해주세요.",
                "제품 선택",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var selectedProduct =
            viewModel.SelectedProduct;

        var confirmed =
            App.Services.Confirmation.ConfirmProductDelete(
                selectedProduct.ProductName);

        if (!confirmed)
        {
            return;
        }

        try
        {
            App.Services.ProductMaster.Delete(
                selectedProduct.Product.Id);

            App.Services.ProductCatalog.Reload();

            viewModel.SelectedProduct = null;

            viewModel.RefreshProducts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "제품 삭제 실패",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
    
    private void AddBoxButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new BoxEditWindow
            {
                Owner = this
            };

        var result =
            window.ShowDialog();

        if (result != true)
        {
            return;
        }

        if (DataContext is not MasterManagementViewModel viewModel)
        {
            return;
        }

        viewModel.RefreshBoxes();
    }
    
    private void EditBoxButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not MasterManagementViewModel viewModel)
        {
            return;
        }

        if (viewModel.SelectedBox is null)
        {
            MessageBox.Show(
                "수정할 박스를 먼저 선택해주세요.",
                "박스 선택",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window =
            new BoxEditWindow(
                viewModel.SelectedBox)
            {
                Owner = this
            };

        var result =
            window.ShowDialog();

        if (result != true)
        {
            return;
        }

        viewModel.SelectedBox = null;

        viewModel.RefreshBoxes();
        viewModel.RefreshProducts();
    }
    
    private void DeleteBoxButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not MasterManagementViewModel viewModel)
        {
            return;
        }

        if (viewModel.SelectedBox is null)
        {
            MessageBox.Show(
                "삭제할 박스를 먼저 선택해주세요.",
                "박스 선택",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var selectedBox =
            viewModel.SelectedBox;

        var confirmed =
            App.Services.Confirmation.ConfirmBoxDelete(
                selectedBox.DisplaySize);

        if (!confirmed)
        {
            return;
        }

        try
        {
            App.Services.BoxMaster.Delete(
                selectedBox.Id);

            App.Services.BoxCatalog.Reload();

            viewModel.SelectedBox = null;

            viewModel.RefreshBoxes();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "박스 삭제 실패",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}