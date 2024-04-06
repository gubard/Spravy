using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class EmptyApplySettings : IApplySettings
{
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }
}