using System.Threading;
using System.Threading.Tasks;
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

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        return Result.SuccessValueTask;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return Result.SuccessValueTask;
    }

    public ValueTask<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.SuccessValueTask;
    }
}