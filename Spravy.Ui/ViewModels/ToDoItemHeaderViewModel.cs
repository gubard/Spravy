using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
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
    public required MainSplitViewModel MainSplitViewModel { get; init; }

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
                if (Item is not null)
                {
                    Item.Name = str;
                }

                return  DialogViewer.CloseInputDialogAsync();
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
        MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen;
    }
}