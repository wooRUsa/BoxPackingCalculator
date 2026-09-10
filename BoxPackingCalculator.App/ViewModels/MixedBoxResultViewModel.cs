using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class MixedBoxResultViewModel
{
    public required int BoxNumber { get; init; }

    public required string BoxDisplaySize { get; init; }

    public required MixedBoxSuggestion Suggestion { get; init; }


    public string BoxTitle =>
        $"혼합박스 #{BoxNumber}";


    public string ItemSummary =>
        string.Join(
            " + ",
            Suggestion.Items.Select(
                item =>
                    $"{item.Product.Name} {item.Quantity}개"));


    public double OccupancyPercent =>
        Suggestion.TotalOccupancyRate * 100;


    public double EstimatedWeightKg =>
        Suggestion.EstimatedWeightKg;
}