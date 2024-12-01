namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public partial class AddPasswordItemViewModel : DialogableViewModelBase
{
    [ObservableProperty]
    private string customAvailableCharacters = string.Empty;

    [ObservableProperty]
    private bool isAvailableLowerLatin = true;

    [ObservableProperty]
    private bool isAvailableNumber = true;

    [ObservableProperty]
    private bool isAvailableSpecialSymbols = true;

    [ObservableProperty]
    private bool isAvailableUpperLatin = true;

    [ObservableProperty]
    private string key = string.Empty;

    [ObservableProperty]
    private ushort length = 512;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string regex = string.Empty;

    public override string ViewId => $"{TypeCache<AddPasswordItemViewModel>.Type}";

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