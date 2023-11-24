using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemHeaderViewModel : ViewModelBase
{
    private ToDoItemViewModel? item;

    public ToDoItemHeaderViewModel()
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        ChangeNameCommand = CreateCommandFromTask(TaskWork.Create(ChangeNameAsync).RunAsync);
    }

    public ICommand SwitchPaneCommand { get; }
    public ICommand ChangeNameCommand { get; }
    public AvaloniaList<ToDoItemCommand> Commands { get; } = new();

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    public ToDoItemViewModel? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    private async Task ChangeNameAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowSingleStringConfirmDialogAsync(
                async str =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    if (Item is not null)
                    {
                        await Item.ToDoService.UpdateToDoItemNameAsync(Item.Id, str, cancellationToken)
                            .ConfigureAwait(false);

                        await Item.RefreshAsync(cancellationToken).ConfigureAwait(false);
                    }
                },
                box =>
                {
                    box.Text = Item?.Name ?? string.Empty;
                    box.Label = "Name";
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private DispatcherOperation SwitchPane()
    {
        return Dispatcher.UIThread.InvokeAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }
}