namespace Spravy.Ui.ViewModels;

public class EditDescriptionContentViewModel : ViewModelBase
{
    public AvaloniaList<DescriptionType> Types { get; } = new(Enum.GetValues<DescriptionType>());

    [Reactive]
    public string Description { get; set; } = string.Empty;

    [Reactive]
    public DescriptionType Type { get; set; }
}