namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase
{
    public DeletePasswordItemViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Reactive]
    public Guid PasswordItemId { get; set; }

    [Reactive]
    public string PasswordItemName { get; set; } = string.Empty;

    [Inject]
    public required IPasswordService PasswordService { get; init; }

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

        return PasswordService.GetPasswordItemAsync(PasswordItemId, cancellationToken)
           .IfSuccessAsync(value => this.InvokeUiBackgroundAsync(() =>
                {
                     PasswordItemName = value.Name;
                     
                     return Result.Success;
                }),
                cancellationToken);
    }
}