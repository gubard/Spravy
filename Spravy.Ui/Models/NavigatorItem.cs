namespace Spravy.Ui.Models;

public class NavigatorItem
{
    public NavigatorItem(INavigatable navigatable, Action<object> setup)
    {
        Navigatable = navigatable;
        Setup = setup;
    }

    public INavigatable Navigatable { get; }
    public Action<object> Setup { get; }
}
