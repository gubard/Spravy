namespace Spravy.Ui.ViewModels;

public partial class MainSplitViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isPaneOpen;

    [ObservableProperty]
    private object? content;

    public MainSplitViewModel(
        PaneViewModel pane,
        IServiceFactory serviceFactory,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Pane = pane;

        InitializedCommand = SpravyCommand.Create(
            _ =>
                this.InvokeUiBackgroundAsync(() =>
                {
                    Content = serviceFactory.CreateService<LoginViewModel>();

                    return Result.Success;
                }),
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public PaneViewModel Pane { get; }
}
