namespace Spravy.Ui.Android.Services;

public class AndroidSingleViewTopLevelControlFactory : IFactory<ISingleViewTopLevelControl>
{
    private readonly ISingleViewTopLevelControl singleView;

    public AndroidSingleViewTopLevelControlFactory(ISingleViewTopLevelControl singleView)
    {
        this.singleView = singleView;
    }

    public Result<ISingleViewTopLevelControl> Create()
    {
        return singleView.ToResult();
    }
}