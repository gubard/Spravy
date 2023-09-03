using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class CompleteToDoItemViewModel : ViewModelBase
{
    private ToDoSubItemNotify? item;

    public CompleteToDoItemViewModel()
    {
        SkipToDoItemCommand = CreateCommandFromTask(SkipToDoItemAsync);
        CompleteToDoItemCommand = CreateCommandFromTask(CompleteToDoItemAsync);
        IncompleteToDoItemCommand = CreateCommandFromTask(IncompleteToDoItemAsync);
        FailToDoItemCommand = CreateCommandFromTask(FailToDoItemAsync);
        ChangeCompleteStatusToDoItemCommand = CreateCommandFromTask(ChangeCompleteStatusToDoItemAsync);
    }

    public ToDoSubItemNotify? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    public ICommand CompleteToDoItemCommand { get; }
    public ICommand IncompleteToDoItemCommand { get; }
    public ICommand SkipToDoItemCommand { get; }
    public ICommand FailToDoItemCommand { get; }
    public ICommand ChangeCompleteStatusToDoItemCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    private async Task FailToDoItemAsync()
    {
        if (Item is null)
        {
            return;
        }

        await ToDoService.FailToDoItemAsync(Item.Id);
        BackCommand.Execute(null);
    }

    private async Task SkipToDoItemAsync()
    {
        if (Item is null)
        {
            return;
        }

        await ToDoService.SkipToDoItemAsync(Item.Id);
        BackCommand.Execute(null);
    }

    private async Task ChangeCompleteStatusToDoItemAsync()
    {
        if (Item is null)
        {
            return;
        }

        if (Item is IIsCompletedToDoItem completed)
        {
            if (completed.IsCompleted)
            {
                await ToDoService.UpdateToDoItemCompleteStatusAsync(Item.Id, false);
            }
            else
            {
                await ToDoService.UpdateToDoItemCompleteStatusAsync(Item.Id, true);
            }
        }
        else
        {
            await ToDoService.UpdateToDoItemCompleteStatusAsync(Item.Id, true);
        }

        BackCommand.Execute(null);
    }

    private async Task CompleteToDoItemAsync()
    {
        if (Item is null)
        {
            return;
        }

        await ToDoService.UpdateToDoItemCompleteStatusAsync(Item.Id, true);
        BackCommand.Execute(null);
    }

    private async Task IncompleteToDoItemAsync()
    {
        if (Item is null)
        {
            return;
        }

        await ToDoService.UpdateToDoItemCompleteStatusAsync(Item.Id, false);
        BackCommand.Execute(null);
    }
}