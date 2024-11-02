namespace Spravy.Ui.Features.Schedule.Models;

public partial class TimerItemNotify : NotifyBase, IObjectParameters
{
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();

    [ObservableProperty]
    private DateTime dueDateTime;

    [ObservableProperty]
    private string name = string.Empty;

    public TimerItemNotify(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (nameParameterName.Span.AreEquals(parameterName))
        {
            return Name.ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }
}
