using BoxPackingCalculator.Core.Models;
using BoxPackingCalculator.Core.Services;

namespace BoxPackingCalculator.Core.Tests;

public class PackingWorkflowTests
{
    [Fact]
    public void Calculate_RealOrderFlow_PreservesFullBoxesAndPacksRemainders()
    {
        // Arrange
        var her = new Product
        {
            Id = "P007",
            Name = "HER(HOT EDGE RING)",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 9.0
        };

        var g61 = new Product
        {
            Id = "P003",
            Name = "G61",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 10.0
        };

        var g52 = new Product
        {
            Id = "P004",
            Name = "G52",
            BoxId = "BOX_050_051_053",
            CapacityPerBox = 10,
            FullBoxWeightKg = 8.0
        };

        var orders = new[]
        {
            new OrderItem
            {
                Product = her,
                Quantity = 27
            },

            new OrderItem
            {
                Product = g61,
                Quantity = 16
            },

            new OrderItem
            {
                Product = g52,
                Quantity = 24
            }
        };

        var packingCalculator = new PackingCalculator();

        var orderPackingCalculator =
            new OrderPackingCalculator(packingCalculator);

        var mixedPackingCalculator =
            new MixedPackingCalculator();

        // Act - 1. 제품별 계산 + 박스 규격별 그룹화
        var groups =
            orderPackingCalculator.Calculate(orders);

        // 이 주문은 모두 같은 박스를 사용한다.
        var group = Assert.Single(groups);

        // Act - 2. 잔량 혼합 추천
        var suggestions =
            mixedPackingCalculator.Calculate(group);

        // Assert - 완박스는 그대로 유지되어야 한다.
        var totalFullBoxCount =
            group.Items.Sum(x => x.FullBoxCount);

        Assert.Equal(5, totalFullBoxCount);

        // HER 2 + G61 1 + G52 2 = 5


        // Assert - 잔량 총수량
        var totalRemainingQuantity =
            group.Items.Sum(x => x.RemainingQuantity);

        Assert.Equal(17, totalRemainingQuantity);


        // Assert - 잔량은 점유율상 2개의 박스가 필요하다.
        Assert.Equal(2, suggestions.Count);


        // Assert - 어떤 추천 박스도 100%를 초과하면 안 된다.
        Assert.All(
            suggestions,
            suggestion =>
                Assert.True(
                    suggestion.TotalOccupancyRate <= 1.0 + 1e-9));


        // Assert - 추천 과정에서 제품 수량이 유실되지 않아야 한다.
        var suggestedHerQuantity = suggestions
            .SelectMany(x => x.Items)
            .Where(x => x.Product.Id == "P007")
            .Sum(x => x.Quantity);

        var suggestedG61Quantity = suggestions
            .SelectMany(x => x.Items)
            .Where(x => x.Product.Id == "P003")
            .Sum(x => x.Quantity);

        var suggestedG52Quantity = suggestions
            .SelectMany(x => x.Items)
            .Where(x => x.Product.Id == "P004")
            .Sum(x => x.Quantity);

        Assert.Equal(7, suggestedHerQuantity);
        Assert.Equal(6, suggestedG61Quantity);
        Assert.Equal(4, suggestedG52Quantity);
    }
}