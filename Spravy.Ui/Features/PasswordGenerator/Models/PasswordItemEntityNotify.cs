namespace Spravy.Ui.Features.PasswordGenerator.Models;

public class PasswordItemEntityNotify : NotifyBase, IPasswordItem, IIdProperty, IParameters
{
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();
    private static readonly ReadOnlyMemory<char> idParameterName = nameof(Id).AsMemory();

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public Guid Id { get; set; }

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
}
