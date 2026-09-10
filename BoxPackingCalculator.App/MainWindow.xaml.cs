using System.Windows;
using System.Windows.Input;
using BoxPackingCalculator.App.ViewModels;
using BoxPackingCalculator.App.Views;

namespace BoxPackingCalculator.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        FitWindowToWorkArea();

        DataContext =
            new MainWindowViewModel();
    }
    
    private void FitWindowToWorkArea()
    {
        var workArea =
            SystemParameters.WorkArea;

        const double margin = 32;

        Width =
            Math.Min(
                1000,
                workArea.Width - margin);

        Height =
            Math.Min(
                800,
                workArea.Height - margin);

        Left =
            workArea.Left
            + (workArea.Width - Width) / 2;

        Top =
            workArea.Top
            + (workArea.Height - Height) / 2;
    }


    private void QuantityTextBox_PreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
        e.Handled =
            e.Text.Any(character =>
                !char.IsDigit(character));
    }


    private void QuantityTextBox_OnPaste(
        object sender,
        DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(
                DataFormats.Text))
        {
            e.CancelCommand();
            return;
        }

        var text =
            e.DataObject.GetData(
                DataFormats.Text) as string;

        if (string.IsNullOrEmpty(text) ||
            text.Any(character =>
                !char.IsDigit(character)))
        {
            e.CancelCommand();
        }
    }
    
    private void MasterManagementButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new MasterManagementWindow
            {
                Owner = this
            };

        window.ShowDialog();
    }
    
    private void PrintPreviewButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not ViewModels.MainWindowViewModel viewModel)
        {
            return;
        }

        if (viewModel.BoxResultGroups.Count == 0)
        {
            MessageBox.Show(
                "먼저 포장 계산을 실행해주세요.",
                "인쇄 미리보기",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window =
            new Views.PackingPrintPreviewWindow(
                viewModel)
            {
                Owner = this
            };

        window.ShowDialog();
    }
}