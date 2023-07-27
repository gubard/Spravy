using System.Windows.Input;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.Core.Ui.Models;
using ExtensionFramework.ReactiveUI.Models;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        InitializedCommand = CreateCommand(Initialized);
        RefreshViewCommand = CreateCommand(RefreshView);
    }

    [Inject]
    public required RoutedViewHost RoutedViewHost { get; init; }

    [Inject]
    public required AppConfiguration Configuration { get; init; }

    public ICommand InitializedCommand { get; }
    public ICommand RefreshViewCommand { get; }

    private void Initialized()
    {
        Navigator.NavigateTo(Configuration.DefaultMainViewType);
    }

    private void RefreshView()
    {
        Navigator.NavigateTo(Configuration.DefaultMainViewType);
    }
}