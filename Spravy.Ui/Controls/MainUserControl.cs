namespace Spravy.Ui.Controls;

public abstract class MainUserControl<T> : UserControl
    where T : ViewModelBase
{
    protected TextBox? DefaultFocusTextBox;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.View = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        DefaultFocusTextBox?.FocusTextBoxUi();
    }

    public T ViewModel
    {
        get => (DataContext as T).ThrowIfNull();
    }
}
