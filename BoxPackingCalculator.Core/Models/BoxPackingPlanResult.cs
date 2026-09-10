namespace BoxPackingCalculator.Core.Models;

public sealed class BoxPackingPlanResult
{
    public Box? Box { get; init; }

    public required IReadOnlyList<OrderPackingLineResult> OrderLines { get; init; }

    public required IReadOnlyList<MixedBoxSuggestion> MixedSuggestions { get; init; }


    public bool IsPackingNotApplicable =>
        OrderLines.Count > 0
        && OrderLines.All(line =>
            line.ProductInfo.Product.IsPackingNotApplicable);


    public string BoxId =>
        IsPackingNotApplicable
            ? Product.NotApplicableBoxId
            : Box?.Id ?? string.Empty;


    public string BoxDisplaySize =>
        IsPackingNotApplicable
            ? "N/A"
            : Box?.DisplaySize ?? "미지정";


    public int TotalFullBoxCount =>
        IsPackingNotApplicable
            ? 0
            : OrderLines.Sum(line =>
                line.FullBoxCount);


    public int MixedBoxCount =>
        IsPackingNotApplicable
            ? 0
            : MixedSuggestions.Count;


    public int TotalBoxCount =>
        IsPackingNotApplicable
            ? 0
            : TotalFullBoxCount + MixedBoxCount;
}