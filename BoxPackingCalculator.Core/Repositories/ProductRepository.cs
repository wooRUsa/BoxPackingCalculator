using System.Text.Json;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Repositories;

public sealed class ProductRepository
{
    private readonly string _filePath;

    public ProductRepository(string filePath)
    {
        _filePath = filePath;
    }

    public IReadOnlyList<Product> Load()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException(
                "Product data file was not found.",
                _filePath);
        }

        var json = File.ReadAllText(_filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var products =
            JsonSerializer.Deserialize<List<Product>>(
                json,
                options);

        if (products is null)
        {
            throw new InvalidOperationException(
                "Failed to deserialize product data.");
        }

        return products;
    }
    
    public void Save(
        IEnumerable<Product> products)
    {
        var productList =
            products.ToList();

        var options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        var json =
            JsonSerializer.Serialize(
                productList,
                options);

        var directory =
            Path.GetDirectoryName(
                _filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(
                directory);
        }

        var tempFilePath =
            _filePath + ".tmp";

        File.WriteAllText(
            tempFilePath,
            json);

        File.Move(
            tempFilePath,
            _filePath,
            overwrite: true);
    }
    
    public IReadOnlyList<Product> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Array.Empty<Product>();
        }
        
        var normalizedKeyword =
            NormalizeForSearch(keyword);
        
        var products = Load();

        return products
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