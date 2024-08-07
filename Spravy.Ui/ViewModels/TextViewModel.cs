namespace Spravy.Ui.ViewModels;

public partial class TextViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isReadOnly;

    [ObservableProperty]
    private string text = string.Empty;

    [ObservableProperty]
    private bool acceptsReturn;

    [ObservableProperty]
    private TextWrapping textWrapping;

    [ObservableProperty]
    private string label = string.Empty;
}
