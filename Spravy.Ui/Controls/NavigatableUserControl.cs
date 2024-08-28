namespace Spravy.Ui.Controls;

public abstract class NavigatableUserControl<T> : UserControl
    where T : NavigatableViewModelBase
{
    protected TextBox? DefaultFocusTextBox;

    public NavigatableUserControl()
    {
        Initialized += async (_, _) =>
        {
            try
            {
                ViewModel.View = this;
                await ViewModel.LoadStateAsync(CancellationToken.None);
            }
            finally
            {
                if (DefaultFocusTextBox is not null)
                {
                    DefaultFocusTextBox.FocusTextBoxUi();
                }
            }
        };

        DetachedFromVisualTree += async (_, _) =>
        {
            await ViewModel.SaveStateAsync(CancellationToken.None);
        };
    }

    public T ViewModel
    {
        get => (DataContext as T).ThrowIfNull();
    }
}
