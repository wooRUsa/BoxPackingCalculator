namespace BoxPackingCalculator.Core.Models;

public sealed class OrderItem
{
    public required Product Product { get; init; }

    public int Quantity { get; init; }
}