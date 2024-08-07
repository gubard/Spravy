namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public partial class DeletePasswordItemViewModel : ViewModelBase, IObjectParameters
{
    private static readonly ReadOnlyMemory<char> passwordItemNameParameterName = nameof(
            PasswordItemName
        )
        .AsMemory();

    [ObservableProperty]
    private Guid passwordItemId;

    [ObservableProperty]
    private string passwordItemName = string.Empty;

    public DeletePasswordItemViewModel()
    {
        PropertyChanged += OnPropertyChanged;
    }

    public Header4Localization DeleteText
    {
        get => new("DeletePasswordItemView.Header", this);
    }

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (passwordItemNameParameterName.Span.AreEquals(parameterName))
        {
            return PasswordItemName.ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PasswordItemName))
        {
            OnPropertyChanged(nameof(DeleteText));
        }
    }
}
