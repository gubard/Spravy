namespace Spravy.Ui.Controls;

public abstract class MainUserControl<T> : UserControl
    where T : ViewModelBase
{
    protected TextBox? DefaultFocusTextBox;

    public MainUserControl()
    {
        Initialized += async (s, _) =>
        {
            try
            {
                ViewModel.View = this;

                if (s is not IDataContextProvider contextProvider)
                {
                    return;
                }

                if (contextProvider.DataContext is not IStateHolder stateHolder)
                {
                    return;
                }

                await stateHolder.LoadStateAsync(CancellationToken.None);
            }
            finally
            {
                if (DefaultFocusTextBox is not null)
                {
                    DefaultFocusTextBox.FocusTextBoxUi();
                }
            }
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
