namespace Spravy.Ui.Features.ToDo.ViewModels;

public class FastAddToDoItemViewModel : ViewModelBase
{
    public FastAddToDoItemViewModel(
        IToDoService toDoService,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Name = string.Empty;

        AddToDoItemCommand = SpravyCommand.Create(cancellationToken => ParentId.IfNotNullStruct(nameof(ParentId))
               .IfSuccessAsync(id =>
                {
                    var options = new AddToDoItemOptions(id, Name, ToDoItemType.Value, string.Empty,
                        DescriptionType.PlainText,
                        new(null));

                    return toDoService.AddToDoItemAsync(options, cancellationToken).ToResultOnlyAsync();
                }, _ =>
                {
                    var options = new AddRootToDoItemOptions(Name, ToDoItemType.Value, new(null), string.Empty,
                        DescriptionType.PlainText);

                    return toDoService.AddRootToDoItemAsync(options, cancellationToken).ToResultOnlyAsync();
                }, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken),
            errorHandler, taskProgressService);
    }

    public SpravyCommand AddToDoItemCommand { get; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public Guid? ParentId { get; set; }
}