using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemHeaderViewModel : ViewModelBase
{
    private ToDoItemViewModel? item;

    public ToDoItemHeaderViewModel()
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        ChangeNameCommand = CreateCommandFromTask(ChangeNameAsync);
    }

    public ICommand SwitchPaneCommand { get; }
    public ICommand ChangeNameCommand { get; }
    public AvaloniaList<ToDoItemCommand> Commands { get; } = new();

    [Inject]
    public required SplitView SplitView { get; init; }

    public ToDoItemViewModel? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    private Task ChangeNameAsync()
    {
        return DialogViewer.ShowSingleStringConfirmDialogAsync(
            str =>
            {
                DialogViewer.CloseDialog();

                if (Item is not null)
                {
                    Item.Name = str;
                }

                return Task.CompletedTask;
            },
            box =>
            {
                box.Text = Item?.Name;
                box.Watermark = "Name";
            }
        );
    }

    private void SwitchPane()
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
    }
}