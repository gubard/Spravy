using System.Threading;
using System.Threading.Tasks;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class EmptyApplySettings : IApplySettings
{
    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}