namespace Spravy.Ui.ViewModels;

public partial class TextViewModel : DialogableViewModelBase
{
    [ObservableProperty]
    private bool acceptsReturn;

    [ObservableProperty]
    private bool isReadOnly;

    [ObservableProperty]
    private string label = string.Empty;

    [ObservableProperty]
    private string text = string.Empty;

    [ObservableProperty]
    private TextWrapping textWrapping;

    public override string ViewId => $"{TypeCache<TextViewModel>.Type}";

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}