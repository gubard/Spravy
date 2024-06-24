namespace Spravy.Ui.Features.ToDo.Models;

public class ActiveToDoItemNotify : NotifyBase
{
    public ActiveToDoItemNotify(Guid id, INavigator navigator, IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        Id = id;
        
        NavigateToCurrentItem =
            SpravyCommand.Create(
                ct => navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = Id, ct),
                errorHandler, taskProgressService);
    }
    
    public SpravyCommand NavigateToCurrentItem { get; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public string Name { get; set; } = string.Empty;
}