namespace Spravy.Ui.Services;

public class RootViewFactory : IRootViewFactory
{
    private readonly MainProgressBarViewModel mainProgressBarViewModel;
    private readonly MainSplitViewModel mainSplitViewModel;
    private readonly MainViewModel mainViewModel;
    private readonly MainWindowModel mainWindowModel;
    private readonly PaneViewModel paneViewModel;
    private readonly SingleViewModel singleViewModel;

    public RootViewFactory(AccountNotify account)
    {
        paneViewModel = new(account);
        mainProgressBarViewModel = new();
        mainSplitViewModel = new(paneViewModel);
        mainViewModel = new(mainProgressBarViewModel, mainSplitViewModel);
        singleViewModel = new(mainViewModel);
        mainWindowModel = new(mainViewModel);
    }

    public MainWindowModel CreateMainWindowModel()
    {
        return mainWindowModel;
    }

    public SingleViewModel CreateSingleViewModel()
    {
        return singleViewModel;
    }

    public MainViewModel CreateMainViewModel()
    {
        return mainViewModel;
    }

    public MainProgressBarViewModel CreateMainProgressBarViewModel()
    {
        return mainProgressBarViewModel;
    }

    public MainSplitViewModel CreateMainSplitViewModel()
    {
        return mainSplitViewModel;
    }

    public PaneViewModel CreatePaneViewModel()
    {
        return paneViewModel;
    }
}