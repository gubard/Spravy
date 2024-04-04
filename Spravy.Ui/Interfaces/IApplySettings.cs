using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface IApplySettings
{
    ValueTask<Result> ApplySettingsAsync(CancellationToken cancellationToken);
}