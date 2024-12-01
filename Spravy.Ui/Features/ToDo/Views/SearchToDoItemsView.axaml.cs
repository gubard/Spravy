namespace Spravy.Ui.Features.ToDo.Views;

public partial class SearchToDoItemsView : NavigatableUserControl<SearchToDoItemsViewModel>
{
    public SearchToDoItemsView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        SearchTextTextBox.Focus();
    }
}