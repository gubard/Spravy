using System;
using System.Threading.Tasks;
using Avalonia.Collections;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Helpers;
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
    public string Name { get; set; }

    [Reactive]
    public ToDoItemType Type { get; set; }

    [Reactive]
    public string Link { get; set; }

    public override string ViewId => TypeCache<ToDoItemContentViewModel>.Type.Name;

    public override void Stop()
    {
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }
}