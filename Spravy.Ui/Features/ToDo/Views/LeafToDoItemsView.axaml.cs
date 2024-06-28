namespace Spravy.Ui.Features.ToDo.Views;

public partial class LeafToDoItemsView : ReactiveUserControl<LeafToDoItemsViewModel>
{
    public LeafToDoItemsView()
    {
        InitializeComponent();
    }

    public LeafToDoItemsViewModel MainViewModel => ViewModel.ThrowIfNull();
}
