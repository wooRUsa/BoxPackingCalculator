using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class BoxResultGroupViewModel
{
    public BoxResultGroupViewModel(
        BoxPackingPlanResult plan)
    {
        BoxId =
            plan.BoxId;

        BoxDisplaySize =
            plan.BoxDisplaySize;
        
        IsPackingNotApplicable =
            plan.IsPackingNotApplicable;

        Products =
            plan.OrderLines
                .Select(line =>
                    new OrderLineResultViewModel
                    {
                        Result = line
                    })
                .ToList();

        TotalFullBoxCount =
            plan.TotalFullBoxCount;

        MixedBoxCount =
            plan.MixedBoxCount;

        TotalBoxCount =
            plan.TotalBoxCount;

        TotalFullBoxWeightKg =
            IsPackingNotApplicable
                ? 0
                : Products.Sum(
                    product =>
                        product.FullBoxTotalWeightKg);
    }


    public string BoxId { get; }

    public string BoxDisplaySize { get; }


    public IReadOnlyList<OrderLineResultViewModel> Products
    {
        get;
    }


    public int TotalFullBoxCount { get; }

    public int MixedBoxCount { get; }

    public int TotalBoxCount { get; }


    // 잔량 및 혼합박스 예상중량은 제외한다.
    // 현장에서 혼합 포장 완료 후 실제 계량한다.
    public double TotalFullBoxWeightKg { get; }
    
    public bool IsPackingNotApplicable { get; }

    public bool HasPackingSummary =>
        !IsPackingNotApplicable;
    
    public double MemoHeight =>
        Products.Count switch
        {
            1 => 60,
            2 => 100,
            3 => 140,
            4 => 180,
            5 => 230,
            6 => 280,
            _ => 320
        };
}