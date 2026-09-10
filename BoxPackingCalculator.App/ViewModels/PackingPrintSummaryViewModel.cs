namespace BoxPackingCalculator.App.ViewModels;

public sealed class PackingPrintSummaryViewModel
{
    public required int OverallFullBoxCount
    {
        get;
        init;
    }

    public required double OverallFullBoxWeightKg
    {
        get;
        init;
    }

    public required int OverallMixedBoxCount
    {
        get;
        init;
    }

    public required IReadOnlyList<MixedBoxResultViewModel> MixedBoxResults
    {
        get;
        init;
    }
}