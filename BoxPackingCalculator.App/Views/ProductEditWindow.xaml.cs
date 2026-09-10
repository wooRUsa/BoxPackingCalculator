using System.Windows;
using BoxPackingCalculator.App.ViewModels;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.Views;

public partial class ProductEditWindow : Window
{
    public ProductEditWindow()
    {
        InitializeComponent();

        DataContext =
            new ProductEditViewModel();
    }
    public ProductEditWindow(
        ProductMasterRowViewModel productRow)
    {
        InitializeComponent();

        Title = "제품 수정";

        DataContext =
            new ProductEditViewModel(
                productRow);
    }


    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not ProductEditViewModel viewModel)
        {
            return;
        }

        try
        {
            viewModel.SaveChanges();

            DialogResult = true;

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "제품 저장 실패",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}