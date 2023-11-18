using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using Ninject;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        InitializedCommand = CreateInitializedCommand(InitializedAsync);
    }

    [Inject]
    public required AppConfiguration Configuration { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    [Inject]
    public required PaneViewModel PaneViewModel { get; init; }

    [Inject]
    public required RoutedViewHost RoutedViewHost { get; init; }

    public ICommand InitializedCommand { get; }

    private Task InitializedAsync()
    {
        MainSplitViewModel.Content = RoutedViewHost;
        MainSplitViewModel.Pane = PaneViewModel;

        return Navigator.NavigateToAsync(Configuration.DefaultMainViewType);
    }
}