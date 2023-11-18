using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddToDoItemViewModel : RoutableViewModelBase
{
    private ToDoSubItemNotify? parent;
    private string name = string.Empty;
    private ToDoItemType type;

    public AddToDoItemViewModel() : base("add-to-do-item")
    {
        InitializedCommand = CreateInitializedCommand(InitializedAsync);
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
    }

    public ICommand InitializedCommand { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required PathViewModel PathViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public ToDoSubItemNotify? Parent
    {
        get => parent;
        set => this.RaiseAndSetIfChanged(ref parent, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private async Task InitializedAsync()
    {
        var item = await ToDoService.GetToDoItemAsync(Parent.ThrowIfNull().Id).ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                PathViewModel.Items.Clear();
                PathViewModel.Items.Add(new RootItem());
                PathViewModel.Items.AddRange(item.Parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
            }
        );
    }
}