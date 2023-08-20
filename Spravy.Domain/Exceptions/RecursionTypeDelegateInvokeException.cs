namespace Spravy.Domain.Exceptions;

public class RecursionTypeDelegateInvokeException : Exception
{
    public RecursionTypeDelegateInvokeException(Type type, Delegate del)
        : base($"{type} contains in parameters {del}.")
    {
        Type = type;
        Delegate = del;
    }

    public Type Type { get; }
    public Delegate Delegate { get; }
}