namespace Spravy.Ui.Features.ToDo.Models;

public class ActiveToDoItemNotify : NotifyBase
{
    public ActiveToDoItemNotify(Guid id, INavigator navigator, IErrorHandler errorHandler)
    {
        Id = id;
        
        NavigateToCurrentItem =
            SpravyCommand.Create(
                cancellationToken => navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = Id, cancellationToken),
                errorHandler);
    }
    
    public SpravyCommand NavigateToCurrentItem { get; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public string Name { get; set; } = string.Empty;
}