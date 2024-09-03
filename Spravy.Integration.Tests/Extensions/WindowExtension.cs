using Spravy.Ui.Controls;

namespace Spravy.Integration.Tests.Extensions;

public static class WindowExtension
{
    public static TWindow CloseErrorDialogHost<TWindow>(this TWindow window)
        where TWindow : Window
    {
        var dialogHost = window.GetErrorDialogHost();
        dialogHost.IsOpen.Should().BeTrue();

        dialogHost
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Border>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<ContentPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Control>()
            .GetControl<Button>(ElementNames.OkButton)
            .ClickOn(window);

        dialogHost.IsOpen.Should().BeFalse();

        return window;
    }

    public static TWindow MustShowError<TWindow>(this TWindow window)
        where TWindow : Window
    {
        window.GetErrorDialogHost().IsOpen.Should().BeTrue();

        return window;
    }

    public static TWindow SetKeyTextInput<TWindow>(this TWindow window, string text)
        where TWindow : Window
    {
        window.SpravyKeyTextInput(text);
        window.RunJobsAll();

        return window;
    }

    public static TWindow KeyHandleQwerty<TWindow>(
        this TWindow window,
        PhysicalKey key,
        RawInputModifiers modifiers
    )
        where TWindow : Window
    {
        window.SpravyKeyPressQwerty(key, modifiers);
        window.RunJobsAll();
        window.SpravyKeyReleaseQwerty(key, modifiers);
        window.RunJobsAll();

        return window;
    }

    public static TWindow SetSize<TWindow>(this TWindow window, double width, double height)
        where TWindow : Window
    {
        window.Width = width;
        window.Height = height;
        window.RunJobsAll();

        return window;
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window)
        where TWindow : Window
    {
        return window.SaveFrame(FileHelper.GetFrameShortFile());
    }

    public static TWindow LogCurrentState<TWindow>(this TWindow window)
        where TWindow : Window
    {
        var controls = window.GetMainControls();
        Console.WriteLine($"Content: {controls.Content}");
        Console.WriteLine($"ContentDialogHost: {controls.ContentDialogHost.IsOpen}");
        Console.WriteLine($"ErrorDialogHost: {controls.ErrorDialogHost.IsOpen}");
        Console.WriteLine($"InputDialogHost: {controls.InputDialogHost.IsOpen}");
        Console.WriteLine($"ProgressDialogHost: {controls.ProgressDialogHost.IsOpen}");

        if (controls.Content is INotifyDataErrorInfo notifyDataErrorInfo)
        {
            Console.WriteLine($"HasError: {notifyDataErrorInfo.HasErrors}");
        }

        switch (controls.ErrorDialogHost.Dialog)
        {
            case InfoViewModel { Content: ExceptionViewModel exception }:
                Console.WriteLine(exception.ToString());

                break;
            case InfoViewModel { Content: ErrorViewModel error }:
                Console.WriteLine(
                    string.Join(Environment.NewLine, error.Errors.Select(x => x.Message))
                );

                break;
        }

        return window;
    }

    public static async Task<TWindow> SaveFrameAsync<TWindow>(this Task<TWindow> task)
        where TWindow : Window
    {
        var window = await task;

        return window.RunJobsAll().SaveFrame(FileHelper.GetFrameShortFile());
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window, FileInfo file)
        where TWindow : Window
    {
        using var bitmap = window.CaptureRenderedFrame().ThrowIfNull();
        file.Directory.ThrowIfNull().Create();
        bitmap.Save(file.FullName);

        return window;
    }

    public static TWindow Show<TWindow>(this TWindow window)
        where TWindow : Window
    {
        window.Show();

        return window;
    }

    public static TWindow ShowWindow<TWindow>(this TWindow window)
        where TWindow : Window
    {
        window.Show();
        window.RunJobsAll();

        return window;
    }

    public static DialogControl GetErrorDialogHost(this Window window)
    {
        return window.GetMainView().GetControl<DialogControl>("ErrorDialogControl");
    }

    public static MainView GetMainView(this Window window)
    {
        return window.GetControl<ContentControl>("MainView").GetContentView<MainView>();
    }

    public static MainSplitView GetMainSplitView(this Window window)
    {
        return window
            .GetControl<ContentControl>("MainView")
            .GetContentView<MainView>()
            .GetControl<ContentControl>("MainSplit")
            .GetContentView<MainSplitView>();
    }

    public static (
        DialogControl ErrorDialogHost,
        DialogControl ProgressDialogHost,
        DialogControl InputDialogHost,
        DialogControl ContentDialogHost,
        object Content
    ) GetMainControls(this Window window)
    {
        var mainView = window.GetMainView();

        return (
            mainView.GetControl<DialogControl>("ErrorDialogControl"),
            mainView.GetControl<DialogControl>("ProgressDialogControl"),
            mainView.GetControl<DialogControl>("InputDialogControl"),
            mainView.GetControl<DialogControl>("ContentDialogControl"),
            mainView.GetControl<ContentControl>("MainSplit").Content.ThrowIfNull()
        );
    }

    public static TView GetCurrentView<TView, TViewModel>(this Window window)
        where TView : UserControl
        where TViewModel : ViewModelBase
    {
        var contentPresenter = window
            .GetMainSplitView()
            .GetControl<SplitView>("MainSplit")
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Children.Last()
            .ThrowIfIsNotCast<Panel>()
            .Children.First()
            .ThrowIfIsNotCast<ContentPresenter>();

        contentPresenter.Content.ThrowIfNull().ThrowIfIsNotCast<TViewModel>();

        return contentPresenter.Child.ThrowIfNull().ThrowIfIsNotCast<TView>();
    }

    public static TView GetContentDialogView<TView, TViewModel>(this Window window)
        where TView : UserControl
        where TViewModel : ViewModelBase
    {
        return window
            .GetErrorDialogHost()
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogControl>()
            .Case(dh => dh.Name.Should().Be("ProgressDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogControl>()
            .Case(dh => dh.Name.Should().Be("InputDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogControl>()
            .Case(dh => dh.Name.Should().Be("ContentDialogHost"))
            .Case(dh => dh.IsOpen.Should().Be(true))
            .Case(dh => dh.Dialog.ThrowIfNull().Case(dc => dc.ThrowIfIsNotCast<TViewModel>()))
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Children.TakeLast(1)
            .Single()
            .ThrowIfIsNotCast<Border>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<ContentPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<TView>();
    }

    public static TView GetErrorDialogView<TView, TViewModel>(this Window window)
        where TView : UserControl
        where TViewModel : ViewModelBase
    {
        return window
            .GetErrorDialogHost()
            .Case(dh => dh.IsOpen.Should().Be(true))
            .Case(dh => dh.Dialog.ThrowIfNull().Case(dc => dc.ThrowIfIsNotCast<TViewModel>()))
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Children.TakeLast(1)
            .Single()
            .ThrowIfIsNotCast<Border>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<ContentPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<TView>();
    }
}
