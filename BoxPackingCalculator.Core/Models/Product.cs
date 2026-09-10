namespace BoxPackingCalculator.Core.Models;

public sealed class Product
{
    public const string NotApplicableBoxId =
        "N/A";


    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string BoxId { get; init; }

    public int CapacityPerBox { get; init; }

    public double FullBoxWeightKg { get; init; }


    public bool IsPackingNotApplicable =>
        string.Equals(
            BoxId,
            NotApplicableBoxId,
            StringComparison.OrdinalIgnoreCase);


    public bool IsPackingInfoComplete =>
        !IsPackingNotApplicable
        && !string.IsNullOrWhiteSpace(BoxId)
        && CapacityPerBox > 0
        && FullBoxWeightKg > 0;


    public bool IsAvailableForOrdering =>
        IsPackingInfoComplete
        || IsPackingNotApplicable;


    public string PackingInfoStatus =>
        IsPackingNotApplicable
            ? "N/A"
            : IsPackingInfoComplete
                ? "완성"
                : "⚠ 포장 정보 미완성";
}