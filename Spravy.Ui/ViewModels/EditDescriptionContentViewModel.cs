namespace Spravy.Ui.ViewModels;

public partial class EditDescriptionContentViewModel : ViewModelBase
{
    private EditDescriptionContentView? editDescriptionContentView;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DescriptionType type;

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
