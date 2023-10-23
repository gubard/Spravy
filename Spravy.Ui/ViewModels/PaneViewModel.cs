using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Ninject;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    public PaneViewModel()
    {
        ToRootToDoItemViewCommand = CreateCommand(ToRootToDoItemView);
        ToSearchViewCommand = CreateCommand(ToSearchView);
        ToTimersViewCommand = CreateCommand(ToTimersView);
        LogoutCommand = CreateCommandFromTask(LogoutAsync);
    }

    public ICommand ToRootToDoItemViewCommand { get; }
    public ICommand ToSearchViewCommand { get; }
    public ICommand ToTimersViewCommand { get; }
    public ICommand LogoutCommand { get; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IKeeper<TokenResult> KeeperTokenResult { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    private async Task LogoutAsync()
    {
        await ObjectStorage.DeleteAsync(FileIds.LoginFileId);
        KeeperTokenResult.Set(default);
        Navigator.NavigateTo<LoginViewModel>();
        SplitView.IsPaneOpen = false;
    }

    private void ToTimersView()
    {
        Navigator.NavigateTo<TimersViewModel>();
        SplitView.IsPaneOpen = false;
    }

    private void ToRootToDoItemView()
    {
        Navigator.NavigateTo<RootToDoItemViewModel>();
        SplitView.IsPaneOpen = false;
    }

    private void ToSearchView()
    {
        Navigator.NavigateTo<SearchViewModel>();
        SplitView.IsPaneOpen = false;
    }
}