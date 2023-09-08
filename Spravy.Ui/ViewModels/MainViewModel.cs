using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Ninject;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        InitializedCommand = CreateCommand(Initialized);
    }

    [Inject]
    public required AppConfiguration Configuration { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    [Inject]
    public required PaneView PaneView { get; init; }

    [Inject]
    public required RoutedViewHost RoutedViewHost { get; init; }

    public ICommand InitializedCommand { get; }

    private void Initialized()
    {
        SplitView.Content = RoutedViewHost;
        SplitView.Pane = PaneView;
        Navigator.NavigateTo(Configuration.DefaultMainViewType);
    }
}