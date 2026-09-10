using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class BoxCatalogServiceTests
{
    [Fact]
    public void Catalog_LoadsBoxesAndFindsTargetBox()
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
                "boxes.json"));

        var repository =
            new BoxRepository(filePath);

        var catalog =
            new BoxCatalogService(repository);

        // Act
        var allBoxes =
            catalog.GetAll();

        var box =
            catalog.FindById("BOX_050_051_053");

        // Assert
        Assert.Equal(11, allBoxes.Count);

        Assert.NotNull(box);

        Assert.Equal(50, box.Width);
        Assert.Equal(51, box.Depth);
        Assert.Equal(53, box.Height);

        Assert.Equal(
            "50×51×53",
            box.DisplaySize);
    }
}