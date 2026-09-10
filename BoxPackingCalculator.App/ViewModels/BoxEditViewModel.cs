using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoxPackingCalculator.Core.Models;

namespace BoxPackingCalculator.App.ViewModels;

public sealed class BoxEditViewModel : INotifyPropertyChanged
{
    private readonly string? _editingBoxId;

    private string _widthText = string.Empty;
    private string _depthText = string.Empty;
    private string _heightText = string.Empty;


    public string WidthText
    {
        get => _widthText;

        set
        {
            if (_widthText == value)
            {
                return;
            }

            _widthText = value;

            OnPropertyChanged();
        }
    }


    public string DepthText
    {
        get => _depthText;

        set
        {
            if (_depthText == value)
            {
                return;
            }

            _depthText = value;

            OnPropertyChanged();
        }
    }


    public string HeightText
    {
        get => _heightText;

        set
        {
            if (_heightText == value)
            {
                return;
            }

            _heightText = value;

            OnPropertyChanged();
        }
    }


    // 신규 등록용
    public BoxEditViewModel()
    {
    }


    // 기존 박스 수정용
    public BoxEditViewModel(
        Box box)
    {
        _editingBoxId =
            box.Id;

        WidthText =
            box.Width.ToString();

        DepthText =
            box.Depth.ToString();

        HeightText =
            box.Height.ToString();
    }


    public void SaveChanges()
    {
        if (!int.TryParse(
                WidthText,
                out var width))
        {
            throw new InvalidOperationException(
                "가로는 정수로 입력해주세요.");
        }

        if (!int.TryParse(
                DepthText,
                out var depth))
        {
            throw new InvalidOperationException(
                "세로는 정수로 입력해주세요.");
        }

        if (!int.TryParse(
                HeightText,
                out var height))
        {
            throw new InvalidOperationException(
                "높이는 정수로 입력해주세요.");
        }


        if (_editingBoxId is null)
        {
            App.Services.BoxMaster.Add(
                width,
                depth,
                height);
        }
        else
        {
            App.Services.BoxMaster.Update(
                _editingBoxId,
                width,
                depth,
                height);
        }


        App.Services.BoxCatalog.Reload();

        // 박스 ID가 변경된 경우
        // 해당 박스를 사용하던 제품들의 BoxId도
        // BoxMasterService가 같이 변경하므로
        // 제품 카탈로그 역시 다시 읽어야 함
        App.Services.ProductCatalog.Reload();
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}