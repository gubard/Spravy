namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : ViewModelBase
{
    private readonly IPasswordService passwordService;

    public PasswordItemSettingsViewModel(
        IPasswordService passwordService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.passwordService = passwordService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand InitializedCommand { get; }

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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return passwordService.GetPasswordItemAsync(Id, ct)
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
            }), ct);
    }
}