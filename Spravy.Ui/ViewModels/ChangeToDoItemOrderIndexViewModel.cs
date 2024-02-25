using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

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

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var items = await ToDoService.GetSiblingsAsync(Id, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                Items.Clear();
                Items.AddRange(Mapper.Map<IEnumerable<ToDoShortItemNotify>>(items));
            }
        );
    }
}