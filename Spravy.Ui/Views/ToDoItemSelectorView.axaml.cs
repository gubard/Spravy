namespace Spravy.Ui.Views;

public partial class ToDoItemSelectorView : ReactiveUserControl<ToDoItemSelectorViewModel>
{
    public ToDoItemSelectorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}