namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : ViewModelBase
{
    public PasswordItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required IPasswordService PasswordService { get; init; }

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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return PasswordService.GetPasswordItemAsync(Id, cancellationToken)
           .IfSuccessAsync(value => this.InvokeUiBackgroundAsync(() =>
            {
                Name = value.Name;
                Regex = value.Regex;
                Key = value.Key;
                Length = value.Length;
                IsAvailableUpperLatin = value.IsAvailableUpperLatin;
                IsAvailableLowerLatin = value.IsAvailableLowerLatin;
                IsAvailableNumber = value.IsAvailableNumber;
                IsAvailableSpecialSymbols = value.IsAvailableSpecialSymbols;
                CustomAvailableCharacters = value.CustomAvailableCharacters;
                
                return Result.Success;
            }), cancellationToken);
    }
}