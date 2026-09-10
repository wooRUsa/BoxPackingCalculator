using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class ProductCatalogServiceTests
{
    [Fact]
    public void Catalog_LoadsOnlyCompleteProductsAndSupportsLookupAndSearch()
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
            new ProductRepository(
                filePath);

        var catalog =
            new ProductCatalogService(
                repository);


        // Act
        var allProducts =
            catalog.GetAll();

        var herById =
            catalog.FindById(
                "P007");

        var r3ById =
            catalog.FindById(
                "P040");

        var incompleteById =
            catalog.FindById(
                "P005");

        var herSearchResults =
            catalog.Search(
                "HOTEDGERING");

        var incompleteSearchResults =
            catalog.Search(
                "SI-362");


        // Assert
        Assert.Equal(
            34,
            allProducts.Count);


        Assert.NotNull(
            herById);

        Assert.Equal(
            "HER(HOT EDGE RING)",
            herById.Name);

        Assert.Equal(
            "BOX_050_051_053",
            herById.BoxId);


        var herBySearch =
            Assert.Single(
                herSearchResults,
                product =>
                    product.Id == "P007");

        Assert.Equal(
            "HER(HOT EDGE RING)",
            herBySearch.Name);


        // R3는 입수량 1로 확정되어 이제 COMPLETE 제품이다.
        Assert.NotNull(
            r3ById);

        Assert.Equal(
            1,
            r3ById.CapacityPerBox);


        // P005는 아직 완박스 무게가 없으므로
        // 계산용 Catalog에서는 노출되면 안 된다.
        Assert.Null(
            incompleteById);

        Assert.DoesNotContain(
            incompleteSearchResults,
            product =>
                product.Id == "P005");


        Assert.All(
            allProducts,
            product =>
                Assert.True(
                    product.IsPackingInfoComplete));
    }
}