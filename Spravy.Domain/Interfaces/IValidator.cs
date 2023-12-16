using System.Diagnostics.CodeAnalysis;

namespace Spravy.Domain.Interfaces;

public interface IValidator<TException, in TValue> where TException : Exception
{
    bool Validate(TValue value, [MaybeNullWhen(true)] out TException exception);
}