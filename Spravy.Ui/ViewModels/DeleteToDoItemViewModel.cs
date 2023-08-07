using System.Threading.Tasks;
using System.Windows.Input;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    private ToDoSubItemNotify? item;

    public DeleteToDoItemViewModel()
    {
        DeleteCommand = CreateCommandFromTask(DeleteAsync);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ICommand DeleteCommand { get; }

    public ToDoSubItemNotify? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    public async Task DeleteAsync()
    {
        await ToDoService.DeleteToDoItemAsync(Item.ThrowIfNull().Id);
        DialogViewer.CloseDialog();
    }
}