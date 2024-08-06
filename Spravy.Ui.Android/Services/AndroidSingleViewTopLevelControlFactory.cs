using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Views;

namespace Spravy.Ui.Android.Services;

public class AndroidSingleViewTopLevelControlFactory : IFactory<ISingleViewTopLevelControl>
{
    private readonly ISingleViewTopLevelControl singleView;

    public AndroidSingleViewTopLevelControlFactory(SingleView singleView)
    {
        this.singleView = singleView;
    }

    public Result<ISingleViewTopLevelControl> Create()
    {
        return singleView.ToResult();
    }
}
