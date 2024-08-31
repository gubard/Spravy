namespace Spravy.Ui.Features.ToDo.Views;

public partial class MultiToDoItemsView : MainUserControl<MultiToDoItemsViewModel>
{
    public const string ContentContentControlName = "content-content-control";

    public MultiToDoItemsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not MultiToDoItemsView view)
            {
                return;
            }

            UiHelper.MultiToDoItemsViewInitialized.Execute(view);
        };
    }
}
