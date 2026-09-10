using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class PackingPlanService
{
    private readonly OrderBatchService _orderBatchService;
    private readonly BoxCatalogService _boxCatalogService;
    private readonly MixedPackingCalculator _mixedPackingCalculator;

    public PackingPlanService(
        OrderBatchService orderBatchService,
        BoxCatalogService boxCatalogService,
        MixedPackingCalculator mixedPackingCalculator)
    {
        _orderBatchService = orderBatchService;
        _boxCatalogService = boxCatalogService;
        _mixedPackingCalculator = mixedPackingCalculator;
    }

public IReadOnlyList<BoxPackingPlanResult> Calculate(
    IEnumerable<(string ProductId, int Quantity)> orders)
{
    var orderLines =
        _orderBatchService.Calculate(orders);

    var plans =
        orderLines
            .GroupBy(line =>
                line.ProductInfo.Product.BoxId)
            .Select(group =>
            {
                var groupLines =
                    group.ToList();

                var isPackingNotApplicable =
                    groupLines.All(line =>
                        line.ProductInfo.Product
                            .IsPackingNotApplicable);

                if (isPackingNotApplicable)
                {
                    return new BoxPackingPlanResult
                    {
                        Box = null,

                        OrderLines = groupLines,

                        MixedSuggestions =
                            Array.Empty<MixedBoxSuggestion>()
                    };
                }

                var box =
                    _boxCatalogService.FindById(
                        group.Key);

                if (box is null)
                {
                    throw new InvalidOperationException(
                        $"Box '{group.Key}' was not found.");
                }

                var packingGroup =
                    new BoxPackingGroup
                    {
                        BoxId = group.Key,

                        Items = groupLines
                            .Select(line =>
                                line.PackingResult)
                            .ToList()
                    };

                var mixedSuggestions =
                    _mixedPackingCalculator.Calculate(
                        packingGroup);

                return new BoxPackingPlanResult
                {
                    Box = box,

                    OrderLines = groupLines,

                    MixedSuggestions =
                        mixedSuggestions
                };
            })
            .OrderBy(plan =>
                plan.IsPackingNotApplicable ? 1 : 0)
            .ThenBy(plan =>
                plan.BoxId)
            .ToList();

    return plans;
}
}