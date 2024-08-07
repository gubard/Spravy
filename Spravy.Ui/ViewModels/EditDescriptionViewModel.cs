namespace Spravy.Ui.ViewModels;

public partial class EditDescriptionViewModel : ViewModelBase
{
    [ObservableProperty]
    private string toDoItemName = string.Empty;

    public EditDescriptionViewModel(EditDescriptionContentViewModel content)
    {
        Content = content;
    }

    public EditDescriptionContentViewModel Content { get; }

    public Result FocusUi()
    {
        return Content.FocusUi();
    }
}
