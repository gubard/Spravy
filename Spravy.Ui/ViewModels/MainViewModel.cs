using System.Windows.Input;
using Avalonia.ReactiveUI;
using Spravy.Domain.Attributes;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        InitializedCommand = CreateCommand(Initialized);
    }

    [Inject]
    public required RoutedViewHost RoutedViewHost { get; init; }

    [Inject]
    public required AppConfiguration Configuration { get; init; }

    public ICommand InitializedCommand { get; }

    private void Initialized()
    {
        Navigator.NavigateTo(Configuration.DefaultMainViewType);
    }
}