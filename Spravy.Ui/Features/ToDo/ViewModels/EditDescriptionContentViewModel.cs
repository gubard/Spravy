namespace Spravy.Ui.ViewModels;

public partial class EditDescriptionContentViewModel : ViewModelBase
{
    private EditDescriptionContentView? editDescriptionContentView;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DescriptionType descriptionType;

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

        editDescriptionContentView.DescriptionTextBox.Focus();

        if (editDescriptionContentView.DescriptionTextBox.Text is null)
        {
            return Result.Success;
        }

        editDescriptionContentView.DescriptionTextBox.CaretIndex = editDescriptionContentView
            .DescriptionTextBox
            .Text
            .Length;

        return Result.Success;
    }
}
