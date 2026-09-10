using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class ProductBoxOption
{
    public required string BoxId { get; init; }

    public required string DisplayText { get; init; }

    public bool IsNotApplicable =>
        string.Equals(
            BoxId,
            Product.NotApplicableBoxId,
            StringComparison.OrdinalIgnoreCase);
}