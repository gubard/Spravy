using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using Ninject;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
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

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        MainSplitViewModel.Content = RoutedViewHost;
        MainSplitViewModel.Pane = PaneViewModel;
        await Navigator.NavigateToAsync(Configuration.DefaultMainViewType, cancellationToken).ConfigureAwait(false);
    }
}