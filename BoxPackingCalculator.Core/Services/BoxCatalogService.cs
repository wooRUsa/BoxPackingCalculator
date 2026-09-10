using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Repositories;

namespace BoxPackingCalculator.Core.Services;

public sealed class BoxCatalogService
{
    private readonly BoxRepository _repository;

    private IReadOnlyList<Box> _boxes;


    public BoxCatalogService(
        BoxRepository repository)
    {
        _repository = repository;
        _boxes = repository.Load();
    }


    public void Reload()
    {
        _boxes =
            _repository.Load();
    }


    public IReadOnlyList<Box> GetAll()
    {
        return _boxes;
    }


    public Box? FindById(string boxId)
    {
        return _boxes.FirstOrDefault(
            box =>
                string.Equals(
                    box.Id,
                    boxId,
                    StringComparison.OrdinalIgnoreCase));
    }
}