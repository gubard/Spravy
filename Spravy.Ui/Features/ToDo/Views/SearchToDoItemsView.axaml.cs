namespace Spravy.Ui.Features.ToDo.Views;

public partial class SearchToDoItemsView : MainUserControl<SearchToDoItemsViewModel>
{
    public const string SearchTextTextBoxName = "SearchTextTextBox";

    public SearchToDoItemsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not RootToDoItemsView view)
            {
                return;
            }

            if (view.DataContext is not RootToDoItemsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        SearchTextTextBox.Focus();
    }
}
