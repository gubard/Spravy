namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class EditDescriptionContentViewModel : ViewModelBase
{
    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DescriptionType descriptionType;
}
