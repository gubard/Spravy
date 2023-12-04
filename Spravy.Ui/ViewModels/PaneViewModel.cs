using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Ninject;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    public PaneViewModel()
    {
        ToRootToDoItemViewCommand = CreateCommandFromTask(TaskWork.Create(ToRootToDoItemViewAsync).RunAsync);
        ToSearchViewCommand = CreateCommandFromTask(TaskWork.Create(ToSearchViewAsync).RunAsync);
        ToTimersViewCommand = CreateCommandFromTask(TaskWork.Create(ToTimersViewAsync).RunAsync);
        LogoutCommand = CreateCommandFromTask(TaskWork.Create(LogoutAsync).RunAsync);
        ToSettingViewCommand = CreateCommandFromTask(TaskWork.Create(ToSettingViewAsync).RunAsync);
    }

    public ICommand ToRootToDoItemViewCommand { get; }
    public ICommand ToSearchViewCommand { get; }
    public ICommand ToTimersViewCommand { get; }
    public ICommand ToSettingViewCommand { get; }
    public ICommand LogoutCommand { get; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IKeeper<TokenResult> KeeperTokenResult { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    private async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await ObjectStorage.DeleteAsync(FileIds.LoginFileId).ConfigureAwait(false);
        KeeperTokenResult.Set(default);
        await Navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken).ConfigureAwait(false);
        await this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = false);
    }

    private async Task ToTimersViewAsync(CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync(ActionHelper<TimersViewModel>.Empty, cancellationToken).ConfigureAwait(false);
        await this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = false);
    }

    private async Task ToRootToDoItemViewAsync(CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
        await this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = false);
    }

    private async Task ToSearchViewAsync(CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync(ActionHelper<SearchViewModel>.Empty, cancellationToken).ConfigureAwait(false);
        await this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = false);
    }
    
    private async Task ToSettingViewAsync(CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync(ActionHelper<SettingViewModel>.Empty, cancellationToken).ConfigureAwait(false);
        await this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = false);
    }
}