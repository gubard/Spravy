using System.Web;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Browser.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Views;

namespace Spravy.Ui.Browser.Services;

public class BrowserSingleViewTopLevelControlFactory : IFactory<ISingleViewTopLevelControl>
{
    private readonly ISingleViewTopLevelControl singleView;
    private readonly ISingleViewTopLevelControl policyView;

    public BrowserSingleViewTopLevelControlFactory(
        ISingleViewTopLevelControl singleView,
        PolicyView policyView
    )
    {
        this.singleView = singleView;
        this.policyView = policyView;
    }

    public Result<ISingleViewTopLevelControl> Create()
    {
        var url = JsWindowInterop.GetCurrentUrl().ToUri();
        var query = HttpUtility.ParseQueryString(url.Query);
        var view = query.Get("view");

        if (view is null)
        {
            return singleView.ToResult();
        }

        if (view.Equals("policy", StringComparison.InvariantCultureIgnoreCase))
        {
            return policyView.ToResult();
        }

        return singleView.ToResult();
    }
}
