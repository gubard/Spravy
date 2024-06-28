namespace Spravy.Domain.Errors;

public class MaxCycleCountReachedError : Error
{
    public static readonly Guid MainId = new("C03E7090-5471-40C0-AD3E-A76378FCD9DC");

    protected MaxCycleCountReachedError()
        : base(MainId) { }

    public MaxCycleCountReachedError(ulong maxCycleCount)
        : base(MainId)
    {
        MaxCycleCount = maxCycleCount;
    }

    public ulong MaxCycleCount { get; protected set; }

    public override string Message
    {
        get => $"Max cycle count {MaxCycleCount} reached";
    }
}
