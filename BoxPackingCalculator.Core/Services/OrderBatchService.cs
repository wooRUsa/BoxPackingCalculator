using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class OrderBatchService
{
    private readonly OrderEntryService _orderEntryService;

    public OrderBatchService(
        OrderEntryService orderEntryService)
    {
        _orderEntryService = orderEntryService;
    }

    public IReadOnlyList<OrderPackingLineResult> Calculate(
        IEnumerable<(string ProductId, int Quantity)> orders)
    {
        return orders
            .Select(order =>
                _orderEntryService.Calculate(
                    order.ProductId,
                    order.Quantity))
            .ToList();
    }
}