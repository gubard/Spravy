namespace Spravy.Ui.Features.ToDo.Views;

public partial class SearchToDoItemsView : ReactiveUserControl<SearchToDoItemsViewModel>
{
    public const string SearchTextTextBoxName = "SearchTextTextBox";

    public SearchToDoItemsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FindControl<TextBox>(SearchTextTextBoxName)?.Focus();
    }
}