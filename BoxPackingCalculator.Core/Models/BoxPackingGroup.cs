namespace BoxPackingCalculator.Core.Models;

public sealed class BoxPackingGroup
{
    public required string BoxId { get; init; }

    public required IReadOnlyList<PackingResult> Items { get; init; }
}