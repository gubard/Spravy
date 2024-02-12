using System;
using System.Threading.Tasks;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.Domain.Helpers;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemContentViewModel : NavigatableViewModelBase
{
    private string name = string.Empty;
    private ToDoItemType type;
    private string url = string.Empty;

    public ToDoItemContentViewModel() : base(true)
    {
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; } = new(Enum.GetValues<ToDoItemType>());

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

    public string Url
    {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

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