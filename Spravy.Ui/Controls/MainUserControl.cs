namespace Spravy.Ui.Controls;

public abstract class MainUserControl<T> : UserControl
    where T : class
{
    public T ViewModel
    {
        get => (DataContext as T).ThrowIfNull();
    }
}
