using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.Core.Services;

public sealed class MixedPackingCalculator
{
    public IReadOnlyList<MixedBoxSuggestion> Calculate(
        BoxPackingGroup group)
    {
        var hasDifferentBoxId =
            group.Items.Any(
                result => result.Product.BoxId != group.BoxId);

        if (hasDifferentBoxId)
        {
            throw new InvalidOperationException(
                "BoxPackingGroup contains a product with a different BoxId.");
        }
        
        var invalidCapacityItem =
            group.Items.FirstOrDefault(
                result => result.Product.CapacityPerBox <= 0);

        if (invalidCapacityItem is not null)
        {
            throw new ArgumentOutOfRangeException(
                nameof(invalidCapacityItem.Product.CapacityPerBox),
                invalidCapacityItem.Product.CapacityPerBox,
                "CapacityPerBox must be greater than zero.");
        }
        
        // 잔량을 제품 1개 단위로 펼친다.
        var remainingUnits = group.Items
            .Where(result => result.RemainingQuantity > 0)
            .SelectMany(result =>
                Enumerable.Range(0, result.RemainingQuantity)
                    .Select(_ => result.Product))
            .OrderByDescending(product =>
                1.0 / product.CapacityPerBox)
            .ToList();

        // 실제 추천 박스를 만들기 위한 임시 목록
        var workingBoxes = new List<List<Product>>();

        foreach (var product in remainingUnits)
        {
            var unitOccupancyRate =
                1.0 / product.CapacityPerBox;

            List<Product>? targetBox = null;

            // 이미 만들어진 박스 중 들어갈 수 있는 첫 번째 박스를 찾는다.
            foreach (var box in workingBoxes)
            {
                var usedOccupancyRate =
                    box.Sum(item =>
                        1.0 / item.CapacityPerBox);

                if (usedOccupancyRate + unitOccupancyRate
                    <= 1.0 + 1e-9)
                {
                    targetBox = box;
                    break;
                }
            }

            // 들어갈 박스가 없다면 새 박스를 만든다.
            if (targetBox is null)
            {
                targetBox = new List<Product>();
                workingBoxes.Add(targetBox);
            }

            targetBox.Add(product);
        }

        // 임시 결과를 MixedBoxSuggestion으로 변환한다.
        var suggestions = workingBoxes
            .Select(box => new MixedBoxSuggestion
            {
                BoxId = group.BoxId,

                Items = box
                    .GroupBy(product => product.Id)
                    .Select(productGroup =>
                        new MixedPackingItem
                        {
                            Product = productGroup.First(),
                            Quantity = productGroup.Count()
                        })
                    .ToList()
            })
            .ToList();

        return suggestions;
    }
}