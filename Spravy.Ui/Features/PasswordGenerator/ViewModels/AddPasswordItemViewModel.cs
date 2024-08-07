namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public partial class AddPasswordItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string regex = string.Empty;

    [ObservableProperty]
    private string key = string.Empty;

    [ObservableProperty]
    private ushort length = 512;

    [ObservableProperty]
    private bool isAvailableUpperLatin = true;

    [ObservableProperty]
    private bool isAvailableLowerLatin = true;

    [ObservableProperty]
    private bool isAvailableNumber = true;

    [ObservableProperty]
    private bool isAvailableSpecialSymbols = true;

    [ObservableProperty]
    private string customAvailableCharacters = string.Empty;
}
