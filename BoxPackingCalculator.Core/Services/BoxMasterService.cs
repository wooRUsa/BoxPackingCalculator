using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Repositories;

namespace BoxPackingCalculator.Core.Services;

public sealed class BoxMasterService
{
    private readonly BoxRepository _boxRepository;
    private readonly ProductRepository _productRepository;


    public BoxMasterService(
        BoxRepository boxRepository,
        ProductRepository productRepository)
    {
        _boxRepository = boxRepository;
        _productRepository = productRepository;
    }


    public IReadOnlyList<Box> GetAll()
    {
        return _boxRepository
            .Load()
            .OrderBy(box => box.Width)
            .ThenBy(box => box.Depth)
            .ThenBy(box => box.Height)
            .ToList();
    }


    public Box Add(
        int width,
        int depth,
        int height)
    {
        ValidateDimensions(
            width,
            depth,
            height);

        NormalizeBaseDimensions(
            width,
            depth,
            out var normalizedWidth,
            out var normalizedDepth);

        var boxes =
            _boxRepository
                .Load()
                .ToList();

        var id =
            GenerateBoxId(
                normalizedWidth,
                normalizedDepth,
                height);

        var alreadyExists =
            boxes.Any(box =>
                string.Equals(
                    box.Id,
                    id,
                    StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                $"Box '{normalizedWidth}×{normalizedDepth}×{height}' already exists.");
        }

        var box =
            new Box
            {
                Id = id,
                Width = normalizedWidth,
                Depth = normalizedDepth,
                Height = height
            };

        boxes.Add(box);

        _boxRepository.Save(
            boxes);

        return box;
    }


    public Box Update(
        string boxId,
        int width,
        int depth,
        int height)
    {
        ValidateDimensions(
            width,
            depth,
            height);

        NormalizeBaseDimensions(
            width,
            depth,
            out var normalizedWidth,
            out var normalizedDepth);

        var boxes =
            _boxRepository
                .Load()
                .ToList();

        var index =
            boxes.FindIndex(box =>
                string.Equals(
                    box.Id,
                    boxId,
                    StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            throw new KeyNotFoundException(
                $"Box '{boxId}' was not found.");
        }

        var newId =
            GenerateBoxId(
                normalizedWidth,
                normalizedDepth,
                height);

        var duplicateExists =
            boxes
                .Where((box, boxIndex) =>
                    boxIndex != index)
                .Any(box =>
                    string.Equals(
                        box.Id,
                        newId,
                        StringComparison.OrdinalIgnoreCase));

        if (duplicateExists)
        {
            throw new InvalidOperationException(
                $"Box '{normalizedWidth}×{normalizedDepth}×{height}' already exists.");
        }

        var oldId =
            boxes[index].Id;

        var updatedBox =
            new Box
            {
                Id = newId,
                Width = normalizedWidth,
                Depth = normalizedDepth,
                Height = height
            };

        boxes[index] =
            updatedBox;

        _boxRepository.Save(
            boxes);

        if (!string.Equals(
                oldId,
                newId,
                StringComparison.OrdinalIgnoreCase))
        {
            UpdateProductBoxReferences(
                oldId,
                newId);
        }

        return updatedBox;
    }


    public void Delete(
        string boxId)
    {
        var products =
            _productRepository.Load();

        var isInUse =
            products.Any(product =>
                string.Equals(
                    product.BoxId,
                    boxId,
                    StringComparison.OrdinalIgnoreCase));

        if (isInUse)
        {
            throw new InvalidOperationException(
                $"Box '{boxId}' is currently used by one or more products and cannot be deleted.");
        }

        var boxes =
            _boxRepository
                .Load()
                .ToList();

        var box =
            boxes.FirstOrDefault(item =>
                string.Equals(
                    item.Id,
                    boxId,
                    StringComparison.OrdinalIgnoreCase));

        if (box is null)
        {
            throw new KeyNotFoundException(
                $"Box '{boxId}' was not found.");
        }

        boxes.Remove(box);

        _boxRepository.Save(
            boxes);
    }


    private void UpdateProductBoxReferences(
        string oldBoxId,
        string newBoxId)
    {
        var products =
            _productRepository
                .Load()
                .ToList();

        var changed = false;

        for (var i = 0; i < products.Count; i++)
        {
            var product =
                products[i];

            if (!string.Equals(
                    product.BoxId,
                    oldBoxId,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            products[i] =
                new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    BoxId = newBoxId,
                    CapacityPerBox = product.CapacityPerBox,
                    FullBoxWeightKg = product.FullBoxWeightKg
                };

            changed = true;
        }

        if (changed)
        {
            _productRepository.Save(
                products);
        }
    }


    private static void ValidateDimensions(
        int width,
        int depth,
        int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                width,
                "Width must be greater than zero.");
        }

        if (depth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(depth),
                depth,
                "Depth must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                height,
                "Height must be greater than zero.");
        }
    }


    private static void NormalizeBaseDimensions(
        int width,
        int depth,
        out int normalizedWidth,
        out int normalizedDepth)
    {
        normalizedWidth =
            Math.Min(
                width,
                depth);

        normalizedDepth =
            Math.Max(
                width,
                depth);
    }


    private static string GenerateBoxId(
        int width,
        int depth,
        int height)
    {
        return
            $"BOX_{width:000}_{depth:000}_{height:000}";
    }
}