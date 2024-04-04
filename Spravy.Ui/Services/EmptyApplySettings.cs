using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class EmptyApplySettings : IApplySettings
{
    public ValueTask<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.SuccessValueTask;
    }
}