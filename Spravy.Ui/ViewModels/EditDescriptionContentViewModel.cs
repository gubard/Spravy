namespace Spravy.Ui.ViewModels;

public class EditDescriptionContentViewModel : ViewModelBase
{
    [Reactive]
    public string Description { get; set; } = string.Empty;

    [Reactive]
    public DescriptionType Type { get; set; }
}
