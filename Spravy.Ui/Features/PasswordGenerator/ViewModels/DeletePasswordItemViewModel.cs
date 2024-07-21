namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase, IParameters
{
    private static readonly ReadOnlyMemory<char> passwordItemNameParameterName = nameof(PasswordItemName).AsMemory();
    
    public DeletePasswordItemViewModel()
    {
        this.WhenAnyValue(x => x.PasswordItemName)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)));
    }

    [Reactive]
    public Guid PasswordItemId { get; set; }

    [Reactive]
    public string PasswordItemName { get; set; } = string.Empty;

    public Header4Localization DeleteText
    {
        get => new("DeletePasswordItemView.Header", this);
    }

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (passwordItemNameParameterName.Span == parameterName)
        {
            return PasswordItemName.ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }
}
