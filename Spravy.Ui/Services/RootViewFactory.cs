namespace Spravy.Ui.Services;

public class RootViewFactory : IRootViewFactory
{
    private readonly MainWindowModel mainWindowModel;
    private readonly MainViewModel mainViewModel;
    private readonly SingleViewModel singleViewModel;
    private readonly MainProgressBarViewModel mainProgressBarViewModel;
    private readonly MainSplitViewModel mainSplitViewModel;
    private readonly PaneViewModel paneViewModel;

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
