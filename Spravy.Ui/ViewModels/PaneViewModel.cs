using System.Threading.Tasks;
using System.Windows.Input;
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
        ToRootToDoItemViewCommand = CreateCommandFromTask(ToRootToDoItemViewAsync);
        ToSearchViewCommand = CreateCommandFromTask(ToSearchViewAsync);
        ToTimersViewCommand = CreateCommandFromTask(ToTimersViewAsync);
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
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    private async Task LogoutAsync()
    {
        await ObjectStorage.DeleteAsync(FileIds.LoginFileId).ConfigureAwait(false);
        KeeperTokenResult.Set(default);
        await Navigator.NavigateToAsync<LoginViewModel>();
        MainSplitViewModel.IsPaneOpen = false;
    }

    private async Task ToTimersViewAsync()
    {
        await Navigator.NavigateToAsync<TimersViewModel>();
        MainSplitViewModel.IsPaneOpen = false;
    }

    private async Task ToRootToDoItemViewAsync()
    {
        await Navigator.NavigateToAsync<RootToDoItemsViewModel>();
        MainSplitViewModel.IsPaneOpen = false;
    }

    private async Task ToSearchViewAsync()
    {
        await Navigator.NavigateToAsync<SearchViewModel>();
        MainSplitViewModel.IsPaneOpen = false;
    }
}