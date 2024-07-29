namespace Spravy.Ui.ViewModels;

public class EditDescriptionViewModel : ViewModelBase
{
    public EditDescriptionViewModel(EditDescriptionContentViewModel content)
    {
        Content = content;
    }

    public EditDescriptionContentViewModel Content { get; }

    [Reactive]
    public string ToDoItemName { get; set; } = string.Empty;

    public Result FocusUi()
    {
        return Content.FocusUi();
    }
}
