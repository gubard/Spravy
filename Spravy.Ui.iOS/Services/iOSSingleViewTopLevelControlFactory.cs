using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.iOS.Services;

public class iOSSingleViewTopLevelControlFactory : IFactory<ISingleViewTopLevelControl>
{
    private readonly ISingleViewTopLevelControl singleView;

    public iOSSingleViewTopLevelControlFactory(ISingleViewTopLevelControl singleView)
    {
        this.singleView = singleView;
    }

    public Result<ISingleViewTopLevelControl> Create()
    {
        return singleView.ToResult();
    }
}