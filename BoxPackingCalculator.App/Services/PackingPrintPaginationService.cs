using System.Windows;
using BoxPackingCalculator.App.ViewModels;
using BoxPackingCalculator.App.Views;

namespace BoxPackingCalculator.App.Services;

public sealed class PackingPrintPaginationService
{
    // A4 세로 - WPF 96 DPI 기준
    private const double PageWidth = 794;
    private const double PageHeight = 1123;

    // PackingPrintView Padding="40"
    private const double HorizontalPadding = 80;
    private const double VerticalPadding = 80;

    // 페이지 상단 "포장 계산 결과" 제목 영역
    private const double TitleAreaHeight = 80;


    private const double ContentWidth =
        PageWidth - HorizontalPadding;

// 실제 WPF 렌더링과 측정값 차이로 인한
// 페이지 하단 잘림 방지용 여유
    private const double PageBottomSafetyMargin = 40;

    private const double AvailableContentHeight =
        PageHeight
        - VerticalPadding
        - TitleAreaHeight
        - PageBottomSafetyMargin;


    public IReadOnlyList<PackingPrintPageViewModel> BuildPages(
        IEnumerable<BoxResultGroupViewModel> groups,
        PackingPrintSummaryViewModel? summary = null,
        string shippingDestination = "",
        string invoiceNumber = "")
    {
        var pages =
            new List<PackingPrintPageViewModel>();

        var currentGroups =
            new List<BoxResultGroupViewModel>();

        double usedHeight = 0;


        foreach (var group in groups)
        {
            var groupHeight =
                MeasureGroupHeight(group);


            // 현재 페이지에 이미 카드가 있고,
            // 새 카드를 넣으면 페이지 높이를 초과하는 경우
            if (currentGroups.Count > 0 &&
                usedHeight + groupHeight > AvailableContentHeight)
            {
                pages.Add(
                    new PackingPrintPageViewModel
                    {
                        PageNumber = pages.Count + 1,
                        Groups = currentGroups.ToList(),
                        Summary = null,
                        
                        ShippingDestination =
                            shippingDestination,

                        InvoiceNumber =
                            invoiceNumber,
                    });

                currentGroups.Clear();

                usedHeight = 0;
            }


            currentGroups.Add(
                group);

            usedHeight +=
                groupHeight;
        }


        // Summary가 없다면 기존 방식대로 마지막 페이지 생성
        if (summary is null)
        {
            if (currentGroups.Count > 0)
            {
                pages.Add(
                    new PackingPrintPageViewModel
                    {
                        PageNumber = pages.Count + 1,
                        Groups = currentGroups.ToList(),
                        Summary = null,
                        
                        ShippingDestination =
                            shippingDestination,

                        InvoiceNumber =
                            invoiceNumber,
                    });
            }

            return pages;
        }


        var summaryHeight =
            MeasureSummaryHeight(
                summary);


        // 규격 카드 자체가 하나도 없는 경우
        if (currentGroups.Count == 0 &&
            pages.Count == 0)
        {
            pages.Add(
                new PackingPrintPageViewModel
                {
                    PageNumber = 1,
                    Groups =
                        Array.Empty<BoxResultGroupViewModel>(),
                    Summary = summary,
                    
                    ShippingDestination =
                        shippingDestination,

                    InvoiceNumber =
                        invoiceNumber,
                });

            return pages;
        }


        // 마지막 규격 카드 페이지에 Summary까지 들어가는 경우
        if (currentGroups.Count > 0 &&
            usedHeight + summaryHeight <= AvailableContentHeight)
        {
            pages.Add(
                new PackingPrintPageViewModel
                {
                    PageNumber = pages.Count + 1,
                    Groups = currentGroups.ToList(),
                    Summary = summary,
                    
                    ShippingDestination =
                        shippingDestination,

                    InvoiceNumber =
                        invoiceNumber,
                });

            return pages;
        }


        // 마지막 규격 카드 페이지에는 공간이 부족함
        if (currentGroups.Count > 0)
        {
            pages.Add(
                new PackingPrintPageViewModel
                {
                    PageNumber = pages.Count + 1,
                    Groups = currentGroups.ToList(),
                    Summary = null,
                    
                    ShippingDestination =
                        shippingDestination,

                    InvoiceNumber =
                        invoiceNumber,
                });
        }


        // Summary 전용 마지막 페이지
        pages.Add(
            new PackingPrintPageViewModel
            {
                PageNumber = pages.Count + 1,
                Groups =
                    Array.Empty<BoxResultGroupViewModel>(),
                Summary = summary,
                
                ShippingDestination =
                    shippingDestination,

                InvoiceNumber =
                    invoiceNumber,
            });


        return pages;
    }


    private static double MeasureGroupHeight(
        BoxResultGroupViewModel group)
    {
        var view =
            new PackingPrintGroupView
            {
                DataContext = group,
                Width = ContentWidth
            };

        // 템플릿과 내부 컨트롤을 먼저 생성
        view.ApplyTemplate();

        // 1차 측정
        view.Measure(
            new Size(
                ContentWidth,
                double.PositiveInfinity));

        var firstHeight =
            view.DesiredSize.Height;

        // DataGrid 등의 실제 레이아웃을 강제로 수행
        view.Arrange(
            new Rect(
                0,
                0,
                ContentWidth,
                firstHeight));

        view.UpdateLayout();

        // 행/바인딩이 실제 생성된 뒤 다시 측정
        view.Measure(
            new Size(
                ContentWidth,
                double.PositiveInfinity));

        return System.Math.Ceiling(
            view.DesiredSize.Height);
    }


    private static double MeasureSummaryHeight(
        PackingPrintSummaryViewModel summary)
    {
        var view =
            new PackingPrintSummaryView
            {
                DataContext = summary,
                Width = ContentWidth
            };


        view.Measure(
            new Size(
                ContentWidth,
                double.PositiveInfinity));


        return view.DesiredSize.Height;
    }
}