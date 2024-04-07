using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    public ChangeToDoItemOrderIndexViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public AvaloniaList<ToDoShortItemNotify> Items { get; } = new();

    [Reactive]
    public ToDoShortItemNotify? SelectedItem { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public bool IsAfter { get; set; } = true;

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetSiblingsAsync(Id, cancellationToken)
            .IfSuccessAsync(
                items => this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            Items.Clear();
                            Items.AddRange(Mapper.Map<ToDoShortItemNotify[]>(items.ToArray()));
                        }
                    )
            );
    }
}