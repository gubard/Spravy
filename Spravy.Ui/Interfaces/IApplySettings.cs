using System.Runtime.CompilerServices;
using System.Threading;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface IApplySettings
{
    ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken);
}