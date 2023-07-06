using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Interfaces;
using Spravy.Core.Models;
using Spravy.Models;

namespace Spravy.ViewModels;

public class AddToDoItemViewModel : RoutableViewModelBase
{
    private ToDoItemNotify? parent;
    private string name = string.Empty;

    public AddToDoItemViewModel() : base("add-to-do-item")
    {
        InitializedCommand = CreateCommandFromTask(InitializedAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
    }

    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required PathControl Path { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ToDoItemNotify? Parent
    {
        get => parent;
        set => this.RaiseAndSetIfChanged(ref parent, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private async Task AddToDoItemAsync()
    {
        var parentValue = Parent.ThrowIfNull();
        var options = new AddToDoItemOptions(parentValue.Id, Name);
        await ToDoService.AddToDoItemAsync(options);
        Navigator.NavigateTo<ToDoItemViewModel>(vm => vm.Id = parentValue.Id);
    }

    private async Task InitializedAsync()
    {
        var item = await ToDoService.GetToDoItemAsync(Parent.ThrowIfNull().Id);
        Path.Items ??= new AvaloniaList<object>();
        Path.Items.Clear();
        Path.Items.Add(new RootItem());
        Path.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
    }
}