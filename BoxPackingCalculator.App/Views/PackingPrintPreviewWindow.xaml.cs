using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using BoxPackingCalculator.App.Services;
using BoxPackingCalculator.App.ViewModels;

namespace BoxPackingCalculator.App.Views;

public partial class PackingPrintPreviewWindow : Window
{
    private readonly IReadOnlyList<PackingPrintPageViewModel> _pages;


    public PackingPrintPreviewWindow(
        MainWindowViewModel viewModel)
    {
        InitializeComponent();

        var paginationService =
            new PackingPrintPaginationService();


        var summary =
            new PackingPrintSummaryViewModel
            {
                OverallFullBoxCount =
                    viewModel.OverallFullBoxCount,

                OverallFullBoxWeightKg =
                    viewModel.OverallFullBoxWeightKg,

                OverallMixedBoxCount =
                    viewModel.OverallMixedBoxCount,

                MixedBoxResults =
                    viewModel.MixedBoxResults.ToList()
            };


        _pages =
            paginationService.BuildPages(
                viewModel.BoxResultGroups,
                summary,
                viewModel.ShippingDestination,
                viewModel.InvoiceNumber);


        PageItemsControl.ItemsSource =
            _pages;
    }


    private void PrintButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_pages.Count == 0)
        {
            MessageBox.Show(
                "인쇄할 포장 결과가 없습니다.",
                "인쇄",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }


        var printDialog =
            new PrintDialog();

        // A4 세로 고정
        printDialog.PrintTicket.PageMediaSize =
            new PageMediaSize(
                PageMediaSizeName.ISOA4);

        printDialog.PrintTicket.PageOrientation =
            PageOrientation.Portrait;


        var confirmed =
            printDialog.ShowDialog();

        if (confirmed != true)
        {
            return;
        }


        var printableWidth =
            printDialog.PrintableAreaWidth;

        var printableHeight =
            printDialog.PrintableAreaHeight;


        var document =
            new FixedDocument();

        document.DocumentPaginator.PageSize =
            new Size(
                printableWidth,
                printableHeight);


        foreach (var page in _pages)
        {
            // 미리보기와 동일한 A4 한 페이지 생성
            var printView =
                new PackingPrintView
                {
                    DataContext = page,

                    // A4 세로 96 DPI 기준
                    Width = 794,
                    Height = 1123
                };


            // 실제 프린터의 인쇄 가능 영역 안에 맞게
            // 비율을 유지하면서 축소
            var viewBox =
                new Viewbox
                {
                    Width = printableWidth,
                    Height = printableHeight,
                    Stretch = Stretch.Uniform,
                    StretchDirection =
                        StretchDirection.DownOnly,
                    Child = printView
                };


            var fixedPage =
                new FixedPage
                {
                    Width = printableWidth,
                    Height = printableHeight,
                    Background = Brushes.White
                };

            fixedPage.Children.Add(
                viewBox);


            var pageContent =
                new PageContent();

            ((IAddChild)pageContent)
                .AddChild(
                    fixedPage);

            document.Pages.Add(
                pageContent);
        }


        printDialog.PrintDocument(
            document.DocumentPaginator,
            "포장 계산 결과");
    }
}