namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemView : MainUserControl<ToDoItemViewModel>
{
    public const string PlainTextTextBlockName = "plain-text-text-block";

    public const string MarkdownTextMarkdownScrollViewerName =
        "markdown-text-markdown-scroll-viewer";

    public ToDoItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemViewModel viewModel)
            {
                return;
            }

            viewModel.Commands.InitializedCommand.Command.Execute(viewModel);
        };
    }
}
