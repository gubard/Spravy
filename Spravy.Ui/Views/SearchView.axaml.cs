namespace Spravy.Ui.Views;

public partial class SearchView : ReactiveUserControl<SearchViewModel>
{
    public const string SearchTextTextBoxName = "SearchTextTextBox";

    public SearchView()
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