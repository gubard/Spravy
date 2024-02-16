using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    private Guid id;
    private bool isAfter = true;
    private ToDoShortItemNotify? selectedItem;

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

    public ToDoShortItemNotify? SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public bool IsAfter
    {
        get => isAfter;
        set => this.RaiseAndSetIfChanged(ref isAfter, value);
    }

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