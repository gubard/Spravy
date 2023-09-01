using System.Windows.Input;
using Avalonia.Controls;
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
    public required AppConfiguration Configuration { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    public ICommand InitializedCommand { get; }

    private void Initialized()
    {
        Navigator.NavigateTo(Configuration.DefaultMainViewType);
    }
}