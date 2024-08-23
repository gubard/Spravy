namespace Spravy.Ui.Controls;

public abstract class MainUserControl<T> : UserControl
    where T : class
{
    public MainUserControl()
    {
        Initialized += (s, e) =>
        {
            if (s is not IDataContextProvider contextProvider)
            {
                return;
            }

            if (contextProvider.DataContext is not IStateHolder stateHolder)
            {
                return;
            }

            stateHolder.LoadStateAsync(CancellationToken.None);
        };

        DetachedFromVisualTree += (s, _) =>
        {
            if (s is not IDataContextProvider contextProvider)
            {
                return;
            }

            if (contextProvider.DataContext is not IStateHolder stateHolder)
            {
                return;
            }

            stateHolder.SaveStateAsync(CancellationToken.None);
        };
    }

    public T ViewModel
    {
        get => (DataContext as T).ThrowIfNull();
    }
}
