using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemHeaderViewModel : ViewModelBase
{
    private ToDoItemViewModel? toDoItemViewModel;

    public ToDoItemHeaderViewModel()
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        ChangeNameCommand = CreateCommandFromTask(TaskWork.Create(ChangeNameAsync).RunAsync);
        ToCurrentToDoItemCommand = CreateCommandFromTask(TaskWork.Create(ToCurrentToDoItemAsync).RunAsync);
    }

    public ICommand SwitchPaneCommand { get; }
    public ICommand ChangeNameCommand { get; }
    public ICommand ToCurrentToDoItemCommand { get; }
    public AvaloniaList<ToDoItemCommand> Commands { get; } = new();
    public ItemsControl? ItemsControlCommands { get; set; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ToDoItemViewModel? ToDoItemViewModel
    {
        get => toDoItemViewModel;
        set => this.RaiseAndSetIfChanged(ref toDoItemViewModel, value);
    }

    private async Task ChangeNameAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowSingleStringConfirmDialogAsync(
                async str =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    if (ToDoItemViewModel is not null)
                    {
                        await ToDoItemViewModel.ToDoService.UpdateToDoItemNameAsync(
                                ToDoItemViewModel.Id,
                                str,
                                cancellationToken
                            )
                            .ConfigureAwait(false);

                        await ToDoItemViewModel.RefreshAsync(cancellationToken).ConfigureAwait(false);
                    }
                },
                box =>
                {
                    box.Text = ToDoItemViewModel?.Name ?? string.Empty;
                    box.Label = "Name";
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ToCurrentToDoItemAsync(CancellationToken cancellationToken)
    {
        var activeToDoItem = await ToDoService.GetCurrentActiveToDoItemAsync(cancellationToken).ConfigureAwait(false);

        if (activeToDoItem.HasValue)
        {
            var item = ToDoItemViewModel.ThrowIfNull();
            item.Id = activeToDoItem.Value.Id;
            await item.RefreshAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private DispatcherOperation SwitchPane()
    {
        return Dispatcher.UIThread.InvokeAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }
}