namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemView : ReactiveUserControl<ToDoItemViewModel>
{
    public const string PlainTextTextBlockName = "plain-text-text-block";
    public const string MarkdownTextMarkdownScrollViewerName =
        "markdown-text-markdown-scroll-viewer";

    public ToDoItemView()
    {
        InitializeComponent();
    }

    public ToDoItemViewModel MainViewModel => ViewModel.ThrowIfNull();
}
