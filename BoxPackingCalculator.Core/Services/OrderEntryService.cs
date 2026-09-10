using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class OrderEntryService
{
    private readonly ProductPackingInfoService _productPackingInfoService;
    private readonly PackingCalculator _packingCalculator;

    public OrderEntryService(
        ProductPackingInfoService productPackingInfoService,
        PackingCalculator packingCalculator)
    {
        _productPackingInfoService = productPackingInfoService;
        _packingCalculator = packingCalculator;
    }

    public OrderPackingLineResult Calculate(
        string productId,
        int quantity)
    {
        var productInfo =
            _productPackingInfoService.FindByProductId(productId);

        if (productInfo is null)
        {
            throw new KeyNotFoundException(
                $"Product '{productId}' was not found.");
        }

        PackingResult packingResult;

        if (productInfo.Product.IsPackingNotApplicable)
        {
            packingResult =
                new PackingResult
                {
                    Product = productInfo.Product,
                    OrderQuantity = quantity,
                    FullBoxCount = 0,
                    RemainingQuantity = 0,
                    RemainingOccupancyRate = 0,
                    EstimatedRemainingWeightKg = 0
                };
        }
        else
        {
            var orderItem =
                new OrderItem
                {
                    Product = productInfo.Product,
                    Quantity = quantity
                };

            packingResult =
                _packingCalculator.Calculate(
                    orderItem);
        }

        return new OrderPackingLineResult
        {
            ProductInfo = productInfo,
            PackingResult = packingResult
        };
    }
}