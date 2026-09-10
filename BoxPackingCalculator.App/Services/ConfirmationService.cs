using System.Windows;

namespace BoxPackingCalculator.App.Services;

public sealed class ConfirmationService
{
    public bool ConfirmReset()
    {
        var result =
            MessageBox.Show(
                "현재 입력한 주문과 계산 결과를 모두 초기화하시겠습니까?",
                "전체 초기화 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
    
    public bool ConfirmProductDelete(
        string productName)
    {
        var result =
            MessageBox.Show(
                $"'{productName}' 제품을 삭제하시겠습니까?\n\n삭제한 제품은 포장 계산 검색 목록에서도 사라집니다.",
                "제품 삭제 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
    
    public bool ConfirmBoxDelete(
        string boxDisplaySize)
    {
        var result =
            MessageBox.Show(
                $"'{boxDisplaySize}' 박스를 삭제하시겠습니까?\n\n" +
                "현재 제품에서 사용 중인 박스는 삭제할 수 없습니다.",
                "박스 삭제 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
}