namespace Spravy.Domain.Exceptions;

public class ErrorsException : Exception
{
    public ErrorsException(ReadOnlyMemory<Error> errors) : base(string.Join(";",
        errors.ToArray().Select(x => $"<{x.Id}>{x.Message}")))
    {
        Errors = errors;
    }

    public ReadOnlyMemory<Error> Errors { get; }
}