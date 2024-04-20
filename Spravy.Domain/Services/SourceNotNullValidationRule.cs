using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class SourceNotNullValidationRule<TObject> : IValidationRule<TObject> where TObject : class
{
    public ConfiguredValueTaskAwaitable<Result> ValidateAsync(TObject? value, string sourceName)
    {
        if (value is null)
        {
            return new Result(new VariableNullValueError(sourceName)).ToValueTaskResult().ConfigureAwait(false);
        }

        return Result.AwaitableFalse;
    }
}