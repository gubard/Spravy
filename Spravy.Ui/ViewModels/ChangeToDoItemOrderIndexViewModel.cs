﻿namespace Spravy.Ui.ViewModels;

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
           .IfSuccessAsync(items => this.InvokeUIBackgroundAsync(() =>
            {
                Items.Clear();
                Items.AddRange(Mapper.Map<ToDoShortItemNotify[]>(items.ToArray()));
            }), cancellationToken);
    }
}