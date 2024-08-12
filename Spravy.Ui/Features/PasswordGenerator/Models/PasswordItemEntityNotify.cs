namespace Spravy.Ui.Features.PasswordGenerator.Models;

public partial class PasswordItemEntityNotify
    : NotifyBase,
        IPasswordItem,
        IIdProperty,
        IObjectParameters
{
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();
    private static readonly ReadOnlyMemory<char> idParameterName = nameof(Id).AsMemory();

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string regex = string.Empty;

    [ObservableProperty]
    private string key = string.Empty;

    [ObservableProperty]
    private string customAvailableCharacters = string.Empty;

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

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (nameParameterName.Span.AreEquals(parameterName))
        {
            return Name.ToResult();
        }

        if (idParameterName.Span.AreEquals(parameterName))
        {
            return Id.ToString().ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }
}
