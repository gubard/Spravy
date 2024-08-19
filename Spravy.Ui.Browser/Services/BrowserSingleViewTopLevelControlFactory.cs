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

        if (url.Segments.Length == 0)
        {
            return singleView.ToResult();
        }

        var lastSegment = url.Segments[^1];

        if (lastSegment.Equals("policy", StringComparison.InvariantCultureIgnoreCase))
        {
            return policyView.ToResult();
        }

        return singleView.ToResult();
    }
}
