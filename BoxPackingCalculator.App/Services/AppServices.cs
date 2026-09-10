using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.App.Services;

public sealed class AppServices
{
    public MasterDataPathService MasterDataPath { get; }


    // Catalog
    public ProductCatalogService ProductCatalog { get; }

    public BoxCatalogService BoxCatalog { get; }


    // Master management
    public ProductMasterService ProductMaster { get; }

    public BoxMasterService BoxMaster { get; }


    // Packing
    public ProductPackingInfoService ProductPackingInfo { get; }

    public PackingPlanService PackingPlan { get; }


    // UI services
    public ConfirmationService Confirmation { get; }


    public AppServices()
    {
        // 실제 운영 마스터 데이터 경로
        var masterDataPath =
            new MasterDataPathService();


        // Repository
        var productRepository =
            new ProductRepository(
                masterDataPath.ProductFilePath);

        var boxRepository =
            new BoxRepository(
                masterDataPath.BoxFilePath);


        // Catalog
        var productCatalog =
            new ProductCatalogService(
                productRepository);

        var boxCatalog =
            new BoxCatalogService(
                boxRepository);


        // Master management
        var productMaster =
            new ProductMasterService(
                productRepository,
                boxRepository);

        var boxMaster =
            new BoxMasterService(
                boxRepository,
                productRepository);


        // Packing
        var productPackingInfoService =
            new ProductPackingInfoService(
                productCatalog,
                boxCatalog);

        var packingCalculator =
            new PackingCalculator();

        var orderEntryService =
            new OrderEntryService(
                productPackingInfoService,
                packingCalculator);

        var orderBatchService =
            new OrderBatchService(
                orderEntryService);

        var mixedPackingCalculator =
            new MixedPackingCalculator();

        var packingPlanService =
            new PackingPlanService(
                orderBatchService,
                boxCatalog,
                mixedPackingCalculator);


        // 외부에 공개
        MasterDataPath =
            masterDataPath;

        ProductCatalog =
            productCatalog;

        BoxCatalog =
            boxCatalog;

        ProductMaster =
            productMaster;

        BoxMaster =
            boxMaster;

        ProductPackingInfo =
            productPackingInfoService;

        PackingPlan =
            packingPlanService;

        Confirmation =
            new ConfirmationService();
    }
}