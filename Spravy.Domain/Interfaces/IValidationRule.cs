namespace Spravy.Domain.Interfaces;

public interface IValidationRule<in TValue>
{
    Cvtar ValidateAsync(TValue value, string sourceName);
}