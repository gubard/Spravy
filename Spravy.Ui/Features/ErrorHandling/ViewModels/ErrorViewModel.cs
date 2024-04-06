using System.Runtime.CompilerServices;
using Avalonia.Collections;
using Spravy.Domain.Errors;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ErrorViewModel : NavigatableViewModelBase
{
    public ErrorViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<ErrorViewModel>.Type.Name;

    public AvaloniaList<Error> ValidationResults { get; } = new();

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting)
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync()
    {
        return Result.AwaitableFalse;
    }
}