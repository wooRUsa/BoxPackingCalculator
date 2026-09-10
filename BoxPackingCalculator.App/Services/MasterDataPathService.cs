using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.Services;

public sealed class MasterDataPathService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };


    public string DataDirectory { get; }


    public string ProductFilePath =>
        Path.Combine(
            DataDirectory,
            "products.json");


    public string BoxFilePath =>
        Path.Combine(
            DataDirectory,
            "boxes.json");


    public MasterDataPathService()
    {
        DataDirectory =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "BoxPackingCalculator",
                "Data");

        Directory.CreateDirectory(
            DataDirectory);


        EnsureSeedFile(
            "products.json");

        EnsureSeedFile(
            "boxes.json");


        RunProductDataMigrations();
    }


    private void EnsureSeedFile(
        string fileName)
    {
        var targetFilePath =
            Path.Combine(
                DataDirectory,
                fileName);

        if (File.Exists(targetFilePath))
        {
            return;
        }


        var sourceFilePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                fileName);

        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException(
                $"Initial master data file '{fileName}' was not found.",
                sourceFilePath);
        }


        File.Copy(
            sourceFilePath,
            targetFilePath,
            overwrite: false);
    }


    private void MergeMissingProductsFromSeed()
    {
        var seedFilePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "products.json");


        if (!File.Exists(seedFilePath))
        {
            throw new FileNotFoundException(
                "Initial product master data file was not found.",
                seedFilePath);
        }


        if (!File.Exists(ProductFilePath))
        {
            return;
        }


        var seedProducts =
            LoadProducts(
                seedFilePath);

        var existingProducts =
            LoadProducts(
                ProductFilePath)
                .ToList();


        var existingProductIds =
            new HashSet<string>(
                existingProducts.Select(
                    product => product.Id),
                StringComparer.OrdinalIgnoreCase);


        var missingProducts =
            seedProducts
                .Where(product =>
                    !existingProductIds.Contains(
                        product.Id))
                .ToList();


        if (missingProducts.Count == 0)
        {
            return;
        }


        existingProducts.AddRange(
            missingProducts);


        var orderedProducts =
            existingProducts
                .OrderBy(product =>
                    GetProductNumber(
                        product.Id))
                .ThenBy(product =>
                    product.Id,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();


        SaveProductsSafely(
            ProductFilePath,
            orderedProducts);
    }


    private static IReadOnlyList<Product> LoadProducts(
        string filePath)
    {
        var json =
            File.ReadAllText(
                filePath);

        return JsonSerializer.Deserialize<List<Product>>(
                   json,
                   JsonOptions)
               ?? new List<Product>();
    }


    private static void SaveProductsSafely(
        string filePath,
        IReadOnlyList<Product> products)
    {
        var json =
            JsonSerializer.Serialize(
                products,
                JsonOptions);


        var temporaryFilePath =
            filePath + ".tmp";


        File.WriteAllText(
            temporaryFilePath,
            json);


        File.Move(
            temporaryFilePath,
            filePath,
            overwrite: true);
    }


    private static int GetProductNumber(
        string productId)
    {
        if (productId.Length > 1
            && productId.StartsWith(
                "P",
                StringComparison.OrdinalIgnoreCase)
            && int.TryParse(
                productId[1..],
                out var number))
        {
            return number;
        }

        return int.MaxValue;
    }
    
    private void RunProductDataMigrations()
    {
        const string migrationFileName =
            "migration_products_v2_complete.flag";


        var migrationFilePath =
            Path.Combine(
                DataDirectory,
                migrationFileName);


        // 이미 이 마이그레이션을 수행한 PC라면 다시 실행하지 않는다.
        if (File.Exists(migrationFilePath))
        {
            return;
        }


        MergeMissingProductsFromSeed();


        // 병합이 정상적으로 끝난 뒤에만 완료 표시를 남긴다.
        File.WriteAllText(
            migrationFilePath,
            DateTime.Now.ToString("O"));
    }
}