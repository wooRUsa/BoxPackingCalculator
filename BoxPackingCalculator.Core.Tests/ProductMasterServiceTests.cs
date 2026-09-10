using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class ProductMasterServiceTests
{
    [Fact]
    public void Add_Update_Delete_Product_PersistsChanges()
    {
        // Arrange
        var tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                "BoxPackingCalculatorTests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(
            tempDirectory);

        var productFilePath =
            Path.Combine(
                tempDirectory,
                "products.json");

        var boxFilePath =
            Path.Combine(
                tempDirectory,
                "boxes.json");

        try
        {
            var productRepository =
                new ProductRepository(
                    productFilePath);

            var boxRepository =
                new BoxRepository(
                    boxFilePath);


            // 테스트용 박스 마스터
            boxRepository.Save(
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
                });


            // 기존 제품 2개
            productRepository.Save(
                new List<Product>
                {
                    new()
                    {
                        Id = "P010",
                        Name = "EXISTING PRODUCT A",
                        BoxId = "BOX_050_051_053",
                        CapacityPerBox = 10,
                        FullBoxWeightKg = 9.0
                    },

                    new()
                    {
                        Id = "P011",
                        Name = "EXISTING PRODUCT B",
                        BoxId = "BOX_050_051_053",
                        CapacityPerBox = 8,
                        FullBoxWeightKg = 10.0
                    }
                });


            var service =
                new ProductMasterService(
                    productRepository,
                    boxRepository);


            // Act 1 - 신규 등록
            var added =
                service.Add(
                    "NEW PRODUCT",
                    "BOX_050_051_053",
                    12,
                    11.5);


            // Assert 1
            Assert.Equal(
                "P012",
                added.Id);

            Assert.Equal(
                "NEW PRODUCT",
                added.Name);

            Assert.Equal(
                12,
                added.CapacityPerBox);

            var afterAdd =
                productRepository.Load();

            Assert.Contains(
                afterAdd,
                product => product.Id == "P012");


            // Act 2 - 기존 제품 수정
            var updated =
                service.Update(
                    "P012",
                    "UPDATED PRODUCT",
                    "BOX_056_058_057",
                    7,
                    15.5);


            // Assert 2
            Assert.Equal(
                "P012",
                updated.Id);

            Assert.Equal(
                "UPDATED PRODUCT",
                updated.Name);

            Assert.Equal(
                "BOX_056_058_057",
                updated.BoxId);

            Assert.Equal(
                7,
                updated.CapacityPerBox);

            Assert.Equal(
                15.5,
                updated.FullBoxWeightKg);

            var afterUpdate =
                productRepository.Load();

            var savedUpdatedProduct =
                Assert.Single(
                    afterUpdate,
                    product => product.Id == "P012");

            Assert.Equal(
                "UPDATED PRODUCT",
                savedUpdatedProduct.Name);

            Assert.Equal(
                "BOX_056_058_057",
                savedUpdatedProduct.BoxId);


            // Act 3 - 삭제
            service.Delete(
                "P012");


            // Assert 3
            var afterDelete =
                productRepository.Load();

            Assert.DoesNotContain(
                afterDelete,
                product => product.Id == "P012");

            Assert.Equal(
                2,
                afterDelete.Count);
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