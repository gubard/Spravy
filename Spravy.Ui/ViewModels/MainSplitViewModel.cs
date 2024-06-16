namespace Spravy.Ui.ViewModels;

public class MainSplitViewModel : ViewModelBase
{
    public MainSplitViewModel(PaneViewModel pane, IServiceFactory serviceFactory, IErrorHandler errorHandler)
    {
        Pane = pane;

        InitializedCommand = SpravyCommand.Create(_ => this.InvokeUiBackgroundAsync(() =>
        {
            Content = serviceFactory.CreateService<LoginViewModel>();

            return Result.Success;
        }), errorHandler);
    }

    public SpravyCommand InitializedCommand { get; }
    public PaneViewModel Pane { get; }

    [Reactive]
    public bool IsPaneOpen { get; set; }

    [Reactive]
    public object? Content { get; set; }
}