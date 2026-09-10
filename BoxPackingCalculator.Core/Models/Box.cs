namespace BoxPackingCalculator.Core.Models;

public sealed class Box
{
    public required string Id { get; init; }

    public int Width { get; init; }

    public int Depth { get; init; }

    public int Height { get; init; }

    public string DisplaySize =>
        $"{Width}×{Depth}×{Height}";
}