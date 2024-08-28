namespace Spravy.Ui.Controls;

public abstract class DialogableUserControl<T> : UserControl
    where T : DialogableViewModelBase
{
    protected TextBox? DefaultFocusTextBox;

    public DialogableUserControl()
    {
        Initialized += (_, _) =>
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
