using System.Windows;
using BoxPackingCalculator.App.ViewModels;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.Views;

public partial class BoxEditWindow : Window
{
    public BoxEditWindow()
    {
        InitializeComponent();

        DataContext =
            new BoxEditViewModel();
    }
    
    public BoxEditWindow(
        Box box)
    {
        InitializeComponent();

        Title = "박스 수정";

        DataContext =
            new BoxEditViewModel(
                box);
    }


    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not BoxEditViewModel viewModel)
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
                "박스 저장 실패",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}