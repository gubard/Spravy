using System;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddRootToDoItemViewModel : RoutableViewModelBase
{
    private string name = string.Empty;
    private ToDoItemType type;

    public AddRootToDoItemViewModel() : base("add-root-to-do")
    {
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }
}