using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Tests;

public class ProductRepositoryTests
{
    [Fact]
    public void Load_ActualProductJson_Loads46ProductsAndStatusesCorrectly()
    {
        // Arrange
        var filePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var repository =
            new ProductRepository(filePath);


        // Act
        var products =
            repository.Load();


        // Assert
        Assert.Equal(
            46,
            products.Count);


        var completeProducts =
            products
                .Where(product =>
                    product.IsPackingInfoComplete)
                .ToList();

        var incompleteProducts =
            products
                .Where(product =>
                    !product.IsPackingInfoComplete)
                .ToList();


        Assert.Equal(
            34,
            completeProducts.Count);

        Assert.Equal(
            12,
            incompleteProducts.Count);


        var her =
            Assert.Single(
                products,
                product =>
                    product.Id == "P007");

        Assert.Equal(
            "HER(HOT EDGE RING)",
            her.Name);

        Assert.Equal(
            "BOX_050_051_053",
            her.BoxId);

        Assert.Equal(
            10,
            her.CapacityPerBox);

        Assert.Equal(
            9.0,
            her.FullBoxWeightKg);

        Assert.True(
            her.IsPackingInfoComplete);


        var r3 =
            Assert.Single(
                products,
                product =>
                    product.Id == "P040");

        Assert.Equal(
            1,
            r3.CapacityPerBox);

        Assert.True(
            r3.IsPackingInfoComplete);


        var incompleteProduct =
            Assert.Single(
                products,
                product =>
                    product.Id == "P005");

        Assert.False(
            incompleteProduct.IsPackingInfoComplete);
    }
    
    [Fact]
    public void Load_AllSpecifiedBoxIdsReferenceExistingBoxes()
    {
        // Arrange
        var productFilePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var boxFilePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "boxes.json"));

        var productRepository =
            new ProductRepository(
                productFilePath);

        var boxRepository =
            new BoxRepository(
                boxFilePath);


        // Act
        var products =
            productRepository.Load();

        var boxes =
            boxRepository.Load();

        var boxIds =
            boxes
                .Select(box =>
                    box.Id)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);


        // Assert
        var productsWithSpecifiedBox =
            products
                .Where(product =>
                    !string.IsNullOrWhiteSpace(
                        product.BoxId))
                .ToList();

        Assert.All(
            productsWithSpecifiedBox,
            product =>
                Assert.Contains(
                    product.BoxId,
                    boxIds));
    }
    
    [Fact]
    public void Search_ByPartialProductName_ReturnsMatchingProduct()
    {
        // Arrange
        var filePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var repository =
            new ProductRepository(filePath);

        // Act
        var results =
            repository.Search("HER");

        // Assert
        var her = Assert.Single(
            results,
            product => product.Id == "P007");

        Assert.Equal(
            "HER(HOT EDGE RING)",
            her.Name);
    }
    
    [Fact]
    public void Search_IsCaseInsensitiveAndReturnsMultipleMatches()
    {
        // Arrange
        var filePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var repository =
            new ProductRepository(filePath);

        // Act
        var lowerCaseResults =
            repository.Search("her");

        var focusResults =
            repository.Search("FOCUS");

        // Assert
        Assert.Contains(
            lowerCaseResults,
            product => product.Id == "P007");

        Assert.Contains(
            focusResults,
            product => product.Name.Contains(
                "FOCUS",
                StringComparison.OrdinalIgnoreCase));

        Assert.True(focusResults.Count >= 2);
    }
    
    [Fact]
    public void Search_IgnoresWhitespaceDifferences()
    {
        // Arrange
        var filePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var repository =
            new ProductRepository(filePath);

        // Act
        var results =
            repository.Search("HOTEDGERING");

        // Assert
        var her = Assert.Single(
            results,
            product => product.Id == "P007");

        Assert.Equal(
            "HER(HOT EDGE RING)",
            her.Name);
    }
    
    [Fact]
    public void Search_IgnoresPunctuationDifferences()
    {
        // Arrange
        var filePath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "BoxPackingCalculator.Core",
                "Data",
                "products.json"));

        var repository =
            new ProductRepository(filePath);

        // Act
        var results =
            repository.Search("UELJK");

        // Assert
        var product = Assert.Single(
            results,
            product => product.Id == "P012");

        Assert.Equal(
            "UEL -JK",
            product.Name);
    }
    
    [Fact]
public void Save_ThenLoad_RestoresProducts()
{
    // Arrange
    var tempDirectory =
        Path.Combine(
            Path.GetTempPath(),
            "BoxPackingCalculatorTests",
            Guid.NewGuid().ToString("N"));

    Directory.CreateDirectory(
        tempDirectory);

    var filePath =
        Path.Combine(
            tempDirectory,
            "products.json");

    try
    {
        var repository =
            new ProductRepository(
                filePath);

        var products =
            new List<Product>
            {
                new()
                {
                    Id = "P900",
                    Name = "TEST PRODUCT A",
                    BoxId = "BOX_050_051_053",
                    CapacityPerBox = 10,
                    FullBoxWeightKg = 9.5
                },

                new()
                {
                    Id = "P901",
                    Name = "TEST PRODUCT B",
                    BoxId = "BOX_056_058_057",
                    CapacityPerBox = 7,
                    FullBoxWeightKg = 15.5
                }
            };


        // Act
        repository.Save(
            products);

        var loadedProducts =
            repository.Load();


        // Assert
        Assert.Equal(
            2,
            loadedProducts.Count);

        var productA =
            Assert.Single(
                loadedProducts,
                product => product.Id == "P900");

        Assert.Equal(
            "TEST PRODUCT A",
            productA.Name);

        Assert.Equal(
            "BOX_050_051_053",
            productA.BoxId);

        Assert.Equal(
            10,
            productA.CapacityPerBox);

        Assert.Equal(
            9.5,
            productA.FullBoxWeightKg);


        var productB =
            Assert.Single(
                loadedProducts,
                product => product.Id == "P901");

        Assert.Equal(
            "TEST PRODUCT B",
            productB.Name);

        Assert.Equal(
            7,
            productB.CapacityPerBox);

        Assert.Equal(
            15.5,
            productB.FullBoxWeightKg);
    }
    finally
    {
        if (Directory.Exists(
                tempDirectory))
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }
}
}