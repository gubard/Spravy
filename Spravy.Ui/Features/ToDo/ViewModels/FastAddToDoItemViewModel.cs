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

        AddToDoItemCommand = SpravyCommand.Create(ct => ParentId.IfNotNullStruct(nameof(ParentId))
               .IfSuccessAsync(id =>
                {
                    var options = new AddToDoItemOptions(id, Name, ToDoItemType.Value, string.Empty,
                        DescriptionType.PlainText,
                        new());

                    return toDoService.AddToDoItemAsync(options, ct).ToResultOnlyAsync();
                }, _ =>
                {
                    var options = new AddRootToDoItemOptions(Name, ToDoItemType.Value, new(), string.Empty,
                        DescriptionType.PlainText);

                    return toDoService.AddRootToDoItemAsync(options, ct).ToResultOnlyAsync();
                }, ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct),
            errorHandler, taskProgressService);
    }

    public SpravyCommand AddToDoItemCommand { get; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public Guid? ParentId { get; set; }
}