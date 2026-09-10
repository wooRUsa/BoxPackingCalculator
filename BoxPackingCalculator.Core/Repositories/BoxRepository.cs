using System.Text.Json;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Repositories;

public sealed class BoxRepository
{
    private readonly string _filePath;

    public BoxRepository(string filePath)
    {
        _filePath = filePath;
    }

    public IReadOnlyList<Box> Load()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException(
                "Box data file was not found.",
                _filePath);
        }

        var json = File.ReadAllText(_filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var boxes =
            JsonSerializer.Deserialize<List<Box>>(
                json,
                options);

        if (boxes is null)
        {
            throw new InvalidOperationException(
                "Failed to deserialize box data.");
        }

        return boxes;
    }
    
    public void Save(
        IEnumerable<Box> boxes)
    {
        var boxList =
            boxes.ToList();

        var options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        var json =
            JsonSerializer.Serialize(
                boxList,
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
}