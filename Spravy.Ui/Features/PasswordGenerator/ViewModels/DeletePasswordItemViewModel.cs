namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase
{
    private readonly IPasswordService passwordService;

    public DeletePasswordItemViewModel(
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
    public Guid PasswordItemId { get; set; }

    [Reactive]
    public string PasswordItemName { get; set; } = string.Empty;

    public Header4Localization DeleteText
    {
        get => new("DeletePasswordItemView.Header", new
        {
            PasswordItemName,
        });
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        this.WhenAnyValue(x => x.PasswordItemName).Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)));

        return passwordService.GetPasswordItemAsync(PasswordItemId, ct)
           .IfSuccessAsync(value => this.InvokeUiBackgroundAsync(() =>
            {
                PasswordItemName = value.Name;

                return Result.Success;
            }), ct);
    }
}