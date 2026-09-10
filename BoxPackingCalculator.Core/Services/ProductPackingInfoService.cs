using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class ProductPackingInfoService
{
    private readonly ProductCatalogService _productCatalog;
    private readonly BoxCatalogService _boxCatalog;


    public ProductPackingInfoService(
        ProductCatalogService productCatalog,
        BoxCatalogService boxCatalog)
    {
        _productCatalog = productCatalog;
        _boxCatalog = boxCatalog;
    }


    public ProductPackingInfo? FindByProductId(
        string productId)
    {
        var product =
            _productCatalog.FindById(productId);

        if (product is null)
        {
            return null;
        }

        return CreatePackingInfo(product);
    }


    public IReadOnlyList<ProductPackingInfo> Search(
        string keyword)
    {
        var products =
            _productCatalog.Search(keyword);

        return products
            .Select(CreatePackingInfo)
            .ToList();
    }


    private ProductPackingInfo CreatePackingInfo(
        Product product)
    {
        if (product.IsPackingNotApplicable)
        {
            return new ProductPackingInfo
            {
                Product = product,
                Box = null
            };
        }

        var box =
            _boxCatalog.FindById(product.BoxId);

        if (box is null)
        {
            throw new InvalidOperationException(
                $"Box '{product.BoxId}' was not found for product '{product.Id}'.");
        }

        return new ProductPackingInfo
        {
            Product = product,
            Box = box
        };
    }
}