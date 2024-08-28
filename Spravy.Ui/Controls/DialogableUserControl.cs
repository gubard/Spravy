namespace Spravy.Ui.Controls;

public abstract class DialogableUserControl<T> : UserControl
    where T : DialogableViewModelBase
{
    protected TextBox? DefaultFocusTextBox;

    public DialogableUserControl()
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
