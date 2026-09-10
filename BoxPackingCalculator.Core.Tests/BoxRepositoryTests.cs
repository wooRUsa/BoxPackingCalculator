using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Tests;

public class BoxRepositoryTests
{
    [Fact]
    public void Load_ActualBoxJson_Loads11BoxesAndTargetBoxCorrectly()
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

        // Act
        var boxes =
            repository.Load();

        // Assert
        Assert.Equal(11, boxes.Count);

        var box = Assert.Single(
            boxes,
            x => x.Id == "BOX_050_051_053");

        Assert.Equal(50, box.Width);
        Assert.Equal(51, box.Depth);
        Assert.Equal(53, box.Height);

        Assert.Equal(
            "50×51×53",
            box.DisplaySize);
    }
    
    [Fact]
public void Save_ThenLoad_RestoresBoxes()
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
            "boxes.json");

    try
    {
        var repository =
            new BoxRepository(
                filePath);

        var boxes =
            new List<Box>
            {
                new()
                {
                    Id = "BOX_050_051_053",
                    Width = 50,
                    Depth = 51,
                    Height = 53
                },

                new()
                {
                    Id = "BOX_056_058_057",
                    Width = 56,
                    Depth = 58,
                    Height = 57
                }
            };


        // Act
        repository.Save(
            boxes);

        var loadedBoxes =
            repository.Load();


        // Assert
        Assert.Equal(
            2,
            loadedBoxes.Count);


        var boxA =
            Assert.Single(
                loadedBoxes,
                box => box.Id == "BOX_050_051_053");

        Assert.Equal(
            50,
            boxA.Width);

        Assert.Equal(
            51,
            boxA.Depth);

        Assert.Equal(
            53,
            boxA.Height);

        Assert.Equal(
            "50×51×53",
            boxA.DisplaySize);


        var boxB =
            Assert.Single(
                loadedBoxes,
                box => box.Id == "BOX_056_058_057");

        Assert.Equal(
            56,
            boxB.Width);

        Assert.Equal(
            58,
            boxB.Depth);

        Assert.Equal(
            57,
            boxB.Height);

        Assert.Equal(
            "56×58×57",
            boxB.DisplaySize);
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