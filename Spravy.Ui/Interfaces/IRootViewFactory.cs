namespace Spravy.Ui.Interfaces;

public interface IRootViewFactory
{
    MainWindowModel CreateMainWindowModel();
    SingleViewModel CreateSingleViewModel();
    MainViewModel CreateMainViewModel();
    MainProgressBarViewModel CreateMainProgressBarViewModel();
    MainSplitViewModel CreateMainSplitViewModel();
    PaneViewModel CreatePaneViewModel();
}