using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class GroupToDoItemSettingsViewModel : NavigatableViewModelBase, IApplySettings
{
    public GroupToDoItemSettingsViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<GroupToDoItemSettingsViewModel>.Type.Name;

    public override void Stop()
    {
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}