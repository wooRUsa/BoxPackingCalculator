using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class PackingCalculator
{
    public PackingResult Calculate(OrderItem order)
    {
        var product = order.Product;
        
        if (product.CapacityPerBox <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(product.CapacityPerBox),
                product.CapacityPerBox,
                "CapacityPerBox must be greater than zero.");
        }
        
        if (order.Quantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order.Quantity),
                order.Quantity,
                "Order quantity cannot be negative.");
        }

        var fullBoxCount =
            order.Quantity / product.CapacityPerBox;

        var remainingQuantity =
            order.Quantity % product.CapacityPerBox;

        var remainingOccupancyRate =
            remainingQuantity == 0
                ? 0
                : (double)remainingQuantity / product.CapacityPerBox;

        var estimatedRemainingWeightKg =
            product.FullBoxWeightKg * remainingOccupancyRate;

        return new PackingResult
        {
            Product = product,
            OrderQuantity = order.Quantity,
            FullBoxCount = fullBoxCount,
            RemainingQuantity = remainingQuantity,
            RemainingOccupancyRate = remainingOccupancyRate,
            EstimatedRemainingWeightKg = estimatedRemainingWeightKg
        };
    }
}