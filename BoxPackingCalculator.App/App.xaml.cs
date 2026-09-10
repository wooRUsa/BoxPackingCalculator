using System.Windows;
using BoxPackingCalculator.App.Services;

namespace BoxPackingCalculator.App;

public partial class App : Application
{
    public static AppServices Services { get; } =
        new AppServices();
}