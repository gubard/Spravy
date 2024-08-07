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
    private string name = string.Empty;

    [ObservableProperty]
    private Guid id;

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
