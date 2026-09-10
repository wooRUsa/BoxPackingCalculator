using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Repositories;

namespace BoxPackingCalculator.Core.Services;

public sealed class ProductCatalogService
{
    private IReadOnlyList<Product> _products =
        Array.Empty<Product>();
    private readonly ProductRepository _repository;

    public ProductCatalogService(
        ProductRepository repository)
    {
        _repository = repository;

        Reload();
    }

    public IReadOnlyList<Product> GetAll()
    {
        return _products;
    }
    
    public void Reload()
    {
        _products =
            _repository
                .Load()
                .Where(product =>
                    product.IsAvailableForOrdering)
                .ToList();
    }

    public Product? FindById(string productId)
    {
        return _products.FirstOrDefault(
            product =>
                string.Equals(
                    product.Id,
                    productId,
                    StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<Product> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Array.Empty<Product>();
        }

        var normalizedKeyword =
            NormalizeForSearch(keyword);

        return _products
            .Where(product =>
                NormalizeForSearch(product.Name)
                    .Contains(
                        normalizedKeyword,
                        StringComparison.OrdinalIgnoreCase))
            .OrderBy(product => product.Name)
            .ToList();
    }

    private static string NormalizeForSearch(string value)
    {
        return new string(
            value
                .Where(char.IsLetterOrDigit)
                .ToArray());
    }
}