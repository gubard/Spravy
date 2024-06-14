namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase
{
    private readonly IPasswordService passwordService;
    
    public DeletePasswordItemViewModel(IPasswordService passwordService, IErrorHandler errorHandler)
    {
        this.passwordService = passwordService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.PasswordItemName).Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)));

        return passwordService.GetPasswordItemAsync(PasswordItemId, cancellationToken)
           .IfSuccessAsync(value => this.InvokeUiBackgroundAsync(() =>
                {
                     PasswordItemName = value.Name;
                     
                     return Result.Success;
                }),
                cancellationToken);
    }
}