namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : ViewModelBase
{
    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public string Regex { get; set; } = string.Empty;

    [Reactive]
    public string Key { get; set; } = string.Empty;

    [Reactive]
    public ushort Length { get; set; } = 512;

    [Reactive]
    public bool IsAvailableUpperLatin { get; set; } = true;

    [Reactive]
    public bool IsAvailableLowerLatin { get; set; } = true;

    [Reactive]
    public bool IsAvailableNumber { get; set; } = true;

    [Reactive]
    public bool IsAvailableSpecialSymbols { get; set; } = true;

    [Reactive]
    public string CustomAvailableCharacters { get; set; } = string.Empty;
}
