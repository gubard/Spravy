using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    public DeleteToDoItemViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }
    
    [Reactive]
    public string ChildrenText { get; set; } = string.Empty;
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return Item.IfNotNull(nameof(Item))
           .IfSuccessAsync(item => Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
                () => ToDoService.GetToDoItemAsync(item.Id, cancellationToken)
                   .IfSuccessAsync(i => this.InvokeUiBackgroundAsync(() => ToDoCache.UpdateUi(i)), cancellationToken)
                   .ToResultOnlyAsync(),
                () => ToDoService.GetParentsAsync(item.Id, cancellationToken)
                   .IfSuccessAsync(
                        parents => this.InvokeUiBackgroundAsync(() => ToDoCache.UpdateParentsUi(item.Id, parents)),
                        cancellationToken), () => ToDoService
                   .ToDoItemToStringAsync(new(Enum.GetValues<ToDoItemStatus>(), item.Id), cancellationToken)
                   .IfSuccessAsync(text => this.InvokeUiBackgroundAsync(() =>
                    {
                        ChildrenText = text;
                        
                        return Result.Success;
                    }), cancellationToken)), cancellationToken);
    }
}