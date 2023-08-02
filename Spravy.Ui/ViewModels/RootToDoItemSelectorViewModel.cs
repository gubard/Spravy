using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemSelectorViewModel : ViewModelBase
{
    public RootToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
    }

    public ICommand InitializedCommand { get; }
    public AvaloniaList<ToDoSubItemNotify> Items { get; } = new();

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }

    private async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetRootToDoSubItemsAsync();
        Items.Clear();
        var source = items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
        Items.AddRange(source.Where(x => x.Status != ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
    }
}