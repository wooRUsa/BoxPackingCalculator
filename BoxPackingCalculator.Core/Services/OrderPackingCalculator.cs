using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class OrderPackingCalculator
{
    private readonly PackingCalculator _packingCalculator;

    public OrderPackingCalculator(PackingCalculator packingCalculator)
    {
        _packingCalculator = packingCalculator;
    }

    public IReadOnlyList<BoxPackingGroup> Calculate(
        IEnumerable<OrderItem> orders)
    {
        var results = orders
            .Select(order => _packingCalculator.Calculate(order));

        var groups = results
            .GroupBy(result => result.Product.BoxId)
            .Select(group => new BoxPackingGroup
            {
                BoxId = group.Key,
                Items = group.ToList()
            })
            .OrderBy(group => group.BoxId)
            .ToList();

        return groups;
    }
}