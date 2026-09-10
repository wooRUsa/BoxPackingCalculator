# BoxPackingCalculator

Windows용 창고 포장 계산기입니다. 주문 수량을 기준으로 제품별 완박스 수량, 잔량, 완박스 중량을 계산하고, 동일한 박스 규격을 사용하는 제품을 그룹화하여 포장 결과와 인쇄용 작업지를 제공합니다.

> Current version: **v1.0.2**

## 주요 기능

- 제품별 주문 수량 입력 및 포장 결과 계산
- 완박스 수량 / 잔량 / 완박스 총중량 계산
- 동일 박스 규격 제품 자동 그룹화
- 잔량 기준 혼합박스 예상
- 제품/박스 마스터 관리
- `N/A` 포장 대상 제품 지원
- 제품 행선지 / 인보이스 번호 입력
- A4 세로 인쇄 미리보기 및 출력
- 제품 수에 따라 가변되는 수기 메모 공간
- 출력 페이지 자동 분할
- 로컬 사용자 데이터 보존

## 포장 계산 규칙

- 이미 완성되는 **완박스는 유지**합니다.
- 혼합 포장은 **같은 박스 규격을 사용하는 제품의 잔량끼리만** 계산합니다.
- 혼합박스는 실제 3D 배치 최적화가 아니라 **입수량 기준 점유율 추정**입니다.
- 혼합박스의 실제 포장 가능 여부와 실제 중량은 작업자가 현장에서 확인합니다.
- 제품별 중량 합계에는 **완박스 중량만 포함**하며, 잔량/혼합박스 예상 중량은 포함하지 않습니다.

## 출력 항목

제품별 출력:
- 제품명
- 주문 수량
- 수량 단위
- 완박스 수량
- 완박스 중량
- 잔량

전체 요약:
- 완박스 총 개수
- 완박스 총중량
- 예상 혼합박스 개수
- 혼합박스 상세

각 박스 규격 카드에는 현장 작업자가 사용할 수 있는 수기 메모 공간이 포함됩니다.

## 기술 스택

- C#
- .NET 10
- WPF
- xUnit
- Inno Setup 6
- JetBrains Rider

## 프로젝트 구조

```text
BoxPackingCalculator/
├─ BoxPackingCalculator.App/          # WPF 애플리케이션
│  ├─ Commands/
│  ├─ Services/
│  ├─ ViewModels/
│  ├─ Views/
│  └─ Assets/
├─ BoxPackingCalculator.Core/         # 계산 / 도메인 / 데이터 처리
│  ├─ Data/
│  ├─ Models/
│  ├─ Repositories/
│  └─ Services/
├─ BoxPackingCalculator.Core.Tests/   # xUnit 테스트
├─ Installer/                         # Inno Setup 스크립트
└─ BoxPackingCalculator.sln
```

## 빌드

```powershell
dotnet build BoxPackingCalculator.sln
```

## 테스트

```powershell
dotnet test BoxPackingCalculator.sln
```

## Release Publish

```powershell
Remove-Item .\publish\win-x64 -Recurse -Force -ErrorAction SilentlyContinue

dotnet publish .\BoxPackingCalculator.App\BoxPackingCalculator.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=false `
  -o .\publish\win-x64
```

Publish 결과의 `BoxPackingCalculator.exe`를 직접 실행할 수 있습니다.

## 설치 프로그램

`Installer/BoxPackingCalculator.iss`를 Inno Setup 6에서 컴파일하면 Windows 설치 프로그램을 생성할 수 있습니다.

현재 설치파일 이름:

```text
제품_포장_계산기_Setup_v1.0.2.exe
```

> 설치 프로그램은 현재 코드 서명이 적용되지 않은 상태입니다. 일부 Windows 11 환경에서 Smart App Control이 서명되지 않은 설치 프로그램을 차단할 수 있습니다.

## 데이터 저장

기본 마스터 데이터:

```text
BoxPackingCalculator.Core/Data/
├─ products.json
└─ boxes.json
```

실행 후 사용자 데이터:

```text
%LOCALAPPDATA%\BoxPackingCalculator\Data
```

프로그램 업데이트 시에도 사용자 마스터 데이터를 별도로 유지할 수 있습니다.

## Notes

혼합 포장 결과는 작업 보조용 추정값입니다. 실제 제품 배치 가능 여부, 포장 상태 및 혼합박스 중량은 현장 작업자가 최종 확인해야 합니다.

## License

현재 별도의 오픈소스 라이선스를 지정하지 않았습니다.
