namespace Spravy.Ui.Features.Schedule.Models;

public class TimerItemNotify : NotifyBase, IObjectParameters
{
    private static readonly ReadOnlyMemory<char> nameParameterName = nameof(Name).AsMemory();

    public TimerItemNotify(Guid id, DateTime dueDateTime, string name)
    {
        Id = id;
        DueDateTime = dueDateTime;
        Name = name;
    }

    public Guid Id { get; }
    public DateTime DueDateTime { get; }
    public string Name { get; }

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
