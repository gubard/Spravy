using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
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

    private Task ChangeNameAsync(CancellationToken cancellationToken)
    {
        return DialogViewer.ShowSingleStringConfirmDialogAsync(
            str =>
            {
                if (Item is not null)
                {
                    Item.Name = str;
                }

                return DialogViewer.CloseInputDialogAsync(cancellationToken);
            },
            box =>
            {
                box.Text = Item?.Name ?? string.Empty;
                box.Label = "Name";
            },
            cancellationToken
        );
    }

    private void SwitchPane()
    {
        MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen;
    }
}