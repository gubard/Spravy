using System.Threading;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface IApplySettings
{
    Task ApplySettingsAsync(CancellationToken cancellationToken);
}