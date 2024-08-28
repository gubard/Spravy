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
            }
            finally
            {
                DefaultFocusTextBox?.FocusTextBoxUi();
            }
        };
    }

    public T ViewModel
    {
        get => (DataContext as T).ThrowIfNull();
    }
}
