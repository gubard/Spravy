namespace Spravy.Ui.ViewModels;

public class EditDescriptionViewModel : ViewModelBase
{
    [Reactive]
    public string ToDoItemName { get; set; } = string.Empty;

    [Inject]
    public required EditDescriptionContentViewModel Content { get; init; }
}