namespace Spravy.Ui.Controls;

public abstract class MainUserControl<T> : UserControl
    where T : class
{
    public MainUserControl()
    {
        Initialized += async (s, e) =>
        {
            if (s is not IDataContextProvider contextProvider)
            {
                return;
            }

            if (contextProvider.DataContext is not IStateHolder stateHolder)
            {
                return;
            }

            await stateHolder.LoadStateAsync(CancellationToken.None);
        };

        DetachedFromVisualTree += async (s, _) =>
        {
            if (s is not IDataContextProvider contextProvider)
            {
                return;
            }

            if (contextProvider.DataContext is not IStateHolder stateHolder)
            {
                return;
            }

            await stateHolder.SaveStateAsync(CancellationToken.None);
        };
    }

    public T ViewModel
    {
        get => (DataContext as T).ThrowIfNull();
    }
}
