using System.Threading.Tasks;
using Spravy.Domain.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class SettingViewModel : NavigatableViewModelBase
{
    public SettingViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<SettingViewModel>.Type.Name;

    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }
}