namespace Spravy.Ui.ViewModels;

public partial class TextViewModel : DialogableViewModelBase
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

    public override string ViewId
    {
        get => $"{TypeCache<TextViewModel>.Type}";
    }

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
