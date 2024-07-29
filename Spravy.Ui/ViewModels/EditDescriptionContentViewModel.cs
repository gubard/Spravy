namespace Spravy.Ui.ViewModels;

public class EditDescriptionContentViewModel : ViewModelBase
{
    private EditDescriptionContentView? editDescriptionContentView;

    public EditDescriptionContentViewModel(
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        InitializedCommand = SpravyCommand.Create<EditDescriptionContentView>(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    [Reactive]
    public string Description { get; set; } = string.Empty;

    [Reactive]
    public DescriptionType Type { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        EditDescriptionContentView view,
        CancellationToken ct
    )
    {
        editDescriptionContentView = view;

        return Result.AwaitableSuccess;
    }

    public Result FocusUi()
    {
        if (editDescriptionContentView is null)
        {
            return Result.Success;
        }

        var textBox = editDescriptionContentView.GetControl<TextBox>("DescriptionTextBox");
        textBox.Focus();

        if (textBox.Text is null)
        {
            return Result.Success;
        }

        textBox.CaretIndex = textBox.Text.Length;

        return Result.Success;
    }
}
