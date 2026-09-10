using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Repositories;

namespace BoxPackingCalculator.Core.Services;

public sealed class ProductMasterService
{
    private readonly ProductRepository _productRepository;
    private readonly BoxRepository _boxRepository;


    public ProductMasterService(
        ProductRepository productRepository,
        BoxRepository boxRepository)
    {
        _productRepository = productRepository;
        _boxRepository = boxRepository;
    }


    public IReadOnlyList<Product> GetAll()
    {
        return _productRepository
            .Load()
            .OrderBy(product => product.Name)
            .ToList();
    }


    public Product Add(
        string name,
        string boxId,
        int capacityPerBox,
        double fullBoxWeightKg)
    {
        Validate(
            name,
            boxId,
            capacityPerBox,
            fullBoxWeightKg);

        var products =
            _productRepository
                .Load()
                .ToList();

        var product =
            new Product
            {
                Id = GenerateNextProductId(products),
                Name = name.Trim(),
                BoxId = boxId.Trim(),
                CapacityPerBox = capacityPerBox,
                FullBoxWeightKg = fullBoxWeightKg
            };

        products.Add(product);

        _productRepository.Save(
            products);

        return product;
    }


    public Product Update(
        string productId,
        string name,
        string boxId,
        int capacityPerBox,
        double fullBoxWeightKg)
    {
        Validate(
            name,
            boxId,
            capacityPerBox,
            fullBoxWeightKg);

        var products =
            _productRepository
                .Load()
                .ToList();

        var index =
            products.FindIndex(
                product =>
                    string.Equals(
                        product.Id,
                        productId,
                        StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            throw new KeyNotFoundException(
                $"Product '{productId}' was not found.");
        }

        var updatedProduct =
            new Product
            {
                Id = products[index].Id,
                Name = name.Trim(),
                BoxId = boxId.Trim(),
                CapacityPerBox = capacityPerBox,
                FullBoxWeightKg = fullBoxWeightKg
            };

        products[index] =
            updatedProduct;

        _productRepository.Save(
            products);

        return updatedProduct;
    }


    public void Delete(
        string productId)
    {
        var products =
            _productRepository
                .Load()
                .ToList();

        var product =
            products.FirstOrDefault(
                item =>
                    string.Equals(
                        item.Id,
                        productId,
                        StringComparison.OrdinalIgnoreCase));

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Product '{productId}' was not found.");
        }

        products.Remove(product);

        _productRepository.Save(
            products);
    }


    private void Validate(
        string name,
        string boxId,
        int capacityPerBox,
        double fullBoxWeightKg)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name cannot be empty.",
                nameof(name));
        }


        // 0은 "미입력" 상태로 허용한다.
        // 음수만 잘못된 값으로 처리한다.
        if (capacityPerBox < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacityPerBox),
                capacityPerBox,
                "CapacityPerBox cannot be negative.");
        }


        // 0은 "미입력" 상태로 허용한다.
        // 음수만 잘못된 값으로 처리한다.
        if (fullBoxWeightKg < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fullBoxWeightKg),
                fullBoxWeightKg,
                "FullBoxWeightKg cannot be negative.");
        }


        // 박스 미지정은 INCOMPLETE 제품에서 허용한다.
        if (string.IsNullOrWhiteSpace(boxId))
        {
            return;
        }
        
        // N/A는 의도적으로 포장 계산에서 제외하는 정상 제품이다.
        if (string.Equals(
                boxId.Trim(),
                Product.NotApplicableBoxId,
                StringComparison.OrdinalIgnoreCase))
        {
            if (capacityPerBox != 0 ||
                fullBoxWeightKg != 0)
            {
                throw new ArgumentException(
                    "N/A product cannot have packing quantity or weight.");
            }

            return;
        }


        // 박스를 지정했다면 실제 BoxMaster에 존재해야 한다.
        var boxExists =
            _boxRepository
                .Load()
                .Any(box =>
                    string.Equals(
                        box.Id,
                        boxId,
                        StringComparison.OrdinalIgnoreCase));

        if (!boxExists)
        {
            throw new KeyNotFoundException(
                $"Box '{boxId}' was not found.");
        }
    }


    private static string GenerateNextProductId(
        IEnumerable<Product> products)
    {
        var maxNumber =
            products
                .Select(product => product.Id)
                .Where(id =>
                    id.StartsWith(
                        "P",
                        StringComparison.OrdinalIgnoreCase))
                .Select(id =>
                {
                    var numberText =
                        id[1..];

                    return int.TryParse(
                        numberText,
                        out var number)
                            ? number
                            : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

        return $"P{maxNumber + 1:000}";
    }
}