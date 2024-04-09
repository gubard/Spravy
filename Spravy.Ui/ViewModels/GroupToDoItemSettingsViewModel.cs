using System.Runtime.CompilerServices;
using System.Threading;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class GroupToDoItemSettingsViewModel : NavigatableViewModelBase, IApplySettings
{
    public GroupToDoItemSettingsViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<GroupToDoItemSettingsViewModel>.Type.Name;

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting, CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync( CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }
}