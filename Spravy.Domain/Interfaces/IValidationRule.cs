using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IValidationRule<in TValue>
{
    ConfiguredValueTaskAwaitable<Result> ValidateAsync(TValue value, string sourceName);
}