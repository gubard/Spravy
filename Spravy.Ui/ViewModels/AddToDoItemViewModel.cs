using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddToDoItemViewModel : ViewModelBase
{
    private string name = string.Empty;
    private ToDoItemType type;

    public AddToDoItemViewModel()
    {
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }

    [Inject]
    public required PathViewModel PathViewModel { get; init; }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
}