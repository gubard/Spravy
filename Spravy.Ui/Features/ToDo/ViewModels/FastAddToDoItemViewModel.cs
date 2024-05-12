namespace Spravy.Ui.Features.ToDo.ViewModels;

public class FastAddToDoItemViewModel : ViewModelBase
{
    public FastAddToDoItemViewModel()
    {
        Name = string.Empty;
        Types = new(Enum.GetValues<ToDoItemType>());
        
        AddToDoItemCommand = CreateCommandFromTask(async () =>
        {
            if (ParentId.HasValue)
            {
                var options = new AddToDoItemOptions(ParentId.Value, Name, Type, string.Empty,
                    DescriptionType.PlainText, null);
                
                await ToDoService.AddToDoItemAsync(options, CancellationToken.None);
            }
            else
            {
                var options = new AddRootToDoItemOptions(Name, Type, null, string.Empty, DescriptionType.PlainText);
                await ToDoService.AddRootToDoItemAsync(options, CancellationToken.None);
            }
            
            await UiApplicationService.RefreshCurrentViewAsync(CancellationToken.None);
        });
    }
    
    public AvaloniaList<ToDoItemType> Types { get; }
    public ICommand AddToDoItemCommand { get; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IUiApplicationService UiApplicationService { get; init; }
    
    [Reactive]
    public string Name { get; set; }
    
    [Reactive]
    public ToDoItemType Type { get; set; }
    
    [Reactive]
    public Guid? ParentId { get; set; }
}