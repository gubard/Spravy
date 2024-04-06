using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Collections;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemContentViewModel : NavigatableViewModelBase
{
    public ToDoItemContentViewModel() : base(true)
    {
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; } = new(Enum.GetValues<ToDoItemType>());

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public ToDoItemType Type { get; set; }

    [Reactive]
    public string Link { get; set; } = string.Empty;

    public override string ViewId => TypeCache<ToDoItemContentViewModel>.Type.Name;

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting)
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync()
    {
        return Result.AwaitableFalse;
    }
}