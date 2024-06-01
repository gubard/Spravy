namespace Spravy.Ui.Features.ToDo.ViewModels;

public class FastAddToDoItemViewModel : ViewModelBase
{
    public FastAddToDoItemViewModel(
        ITaskProgressService taskProgressService,
        IToDoService toDoService,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler
    )
    {
        Name = string.Empty;
        
        AddToDoItemCommand = SpravyCommand.Create(cancellationToken => taskProgressService.RunProgressAsync(() =>
                ParentId.IfNotNullStruct(nameof(ParentId))
                   .IfSuccessAsync(id =>
                    {
                        var options = new AddToDoItemOptions(id, Name, ToDoItemType.Value, string.Empty,
                            DescriptionType.PlainText, null);
                        
                        return toDoService.AddToDoItemAsync(options, cancellationToken).ToResultOnlyAsync();
                    }, _ =>
                    {
                        var options = new AddRootToDoItemOptions(Name, ToDoItemType.Value, null, string.Empty,
                            DescriptionType.PlainText);
                        
                        return toDoService.AddRootToDoItemAsync(options, cancellationToken).ToResultOnlyAsync();
                    }, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken),
            cancellationToken), errorHandler);
    }
    
    public SpravyCommand AddToDoItemCommand { get; }
    
    [Reactive]
    public string Name { get; set; }
    
    [Reactive]
    public Guid? ParentId { get; set; }
}