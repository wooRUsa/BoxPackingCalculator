using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Repositories;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class BoxMasterServiceTests
{
    [Fact]
    public void Add_Update_Delete_HandlesBoxMasterRules()
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


            // 기존 박스 마스터
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


            // 첫 번째 박스를 사용하는 제품
            productRepository.Save(
                new List<Product>
                {
                    new()
                    {
                        Id = "P001",
                        Name = "TEST PRODUCT",
                        BoxId = "BOX_050_051_053",
                        CapacityPerBox = 10,
                        FullBoxWeightKg = 9.0
                    }
                });


            var service =
                new BoxMasterService(
                    boxRepository,
                    productRepository);


            // Act 1 - 신규 박스 등록
            // 60×55×40으로 입력하지만
            // 앞의 두 치수는 55×60으로 정규화되어야 함
            var added =
                service.Add(
                    60,
                    55,
                    40);


            // Assert 1
            Assert.Equal(
                "BOX_055_060_040",
                added.Id);

            Assert.Equal(
                55,
                added.Width);

            Assert.Equal(
                60,
                added.Depth);

            Assert.Equal(
                40,
                added.Height);

            Assert.Equal(
                "55×60×40",
                added.DisplaySize);


            // Act 2 - 기존 50×51×53 박스 규격 변경
            var updated =
                service.Update(
                    "BOX_050_051_053",
                    54,
                    52,
                    53);


            // Assert 2
            Assert.Equal(
                "BOX_052_054_053",
                updated.Id);

            Assert.Equal(
                "52×54×53",
                updated.DisplaySize);


            // 해당 박스를 사용하던 제품의 BoxId도
            // 자동으로 변경되었는지 확인
            var productsAfterUpdate =
                productRepository.Load();

            var product =
                Assert.Single(
                    productsAfterUpdate);

            Assert.Equal(
                "BOX_052_054_053",
                product.BoxId);


            // Act & Assert 3
            // 현재 제품이 사용 중인 박스이므로 삭제 불가
            Assert.Throws<InvalidOperationException>(
                () => service.Delete(
                    "BOX_052_054_053"));


            // 사용되지 않는 신규 박스는 삭제 가능
            service.Delete(
                "BOX_055_060_040");

            var boxesAfterDelete =
                boxRepository.Load();

            Assert.DoesNotContain(
                boxesAfterDelete,
                box =>
                    box.Id ==
                    "BOX_055_060_040");

            Assert.Contains(
                boxesAfterDelete,
                box =>
                    box.Id ==
                    "BOX_052_054_053");
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