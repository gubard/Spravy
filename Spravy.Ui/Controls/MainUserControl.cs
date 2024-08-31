namespace Spravy.Ui.Controls;

public abstract class MainUserControl<T> : UserControl
    where T : ViewModelBase
{
    protected TextBox? DefaultFocusTextBox;

    public MainUserControl()
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