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
            .Children.OfType<DialogOverlayPopupHost>()
            .Single()
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

        switch (controls.ErrorDialogHost.DialogContent)
        {
            case InfoViewModel { Content: ExceptionViewModel exception, }:
                Console.WriteLine(exception.Exception?.ToString());

                break;
            case InfoViewModel { Content: ErrorViewModel error, }:
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

    public static DialogHost GetErrorDialogHost(this Window window)
    {
        var errorDialogHost = window
            .ThrowIfIsNotCast<MainWindow>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Panel>()
            .Children.Last()
            .ThrowIfIsNotCast<VisualLayerManager>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<ContentPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Panel>()
            .Children.Last()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<MainView>()
            .Case(w => w.DataContext.ThrowIfNull().ThrowIfIsNotCast<MainViewModel>())
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<Panel>()
            .Children.Last()
            .ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ErrorDialogHost"));

        return errorDialogHost;
    }

    public static (
        DialogHost ErrorDialogHost,
        DialogHost ProgressDialogHost,
        DialogHost InputDialogHost,
        DialogHost ContentDialogHost,
        object Content
    ) GetMainControls(this Window window)
    {
        var errorDialogHost = window.GetErrorDialogHost();

        var progressDialogHost = errorDialogHost
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ProgressDialogHost"));

        var inputDialogHost = progressDialogHost
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("InputDialogHost"));

        var contentDialogHost = inputDialogHost
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ContentDialogHost"));

        var content = contentDialogHost
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<ContentControl>()
            .GetContentView<MainSplitView>()
            .Case(w => w.DataContext.ThrowIfNull().ThrowIfIsNotCast<MainSplitViewModel>())
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<SplitView>()
            .Case(sv => sv.Pane.ThrowIfNull().ThrowIfIsNotCast<PaneViewModel>())
            .Content.ThrowIfNull();

        return (errorDialogHost, progressDialogHost, inputDialogHost, contentDialogHost, content);
    }

    public static TView GetCurrentView<TView, TViewModel>(this Window window)
        where TView : UserControl
        where TViewModel : ViewModelBase
    {
        return window
            .GetErrorDialogHost()
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ProgressDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("InputDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ContentDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<ContentControl>()
            .GetContentView<MainSplitView>()
            .Case(w => w.DataContext.ThrowIfNull().ThrowIfIsNotCast<MainSplitViewModel>())
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<SplitView>()
            .Case(sv => sv.Pane.ThrowIfNull().ThrowIfIsNotCast<PaneViewModel>())
            .Case(sv => sv.Content.ThrowIfNull().ThrowIfIsNotCast<TViewModel>())
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Case(g =>
                g.Children.Should()
                    .HaveCount(2)
                    .And.Subject.First()
                    .ThrowIfIsNotCast<Panel>()
                    .Children.Should()
                    .HaveCount(2)
                    .And.Subject.First()
                    .ThrowIfIsNotCast<ContentPresenter>()
                    .Child.ThrowIfNull()
                    .ThrowIfIsNotCast<PaneView>()
            )
            .Children.Skip(1)
            .Single()
            .ThrowIfIsNotCast<Panel>()
            .Children.Should()
            .HaveCount(2)
            .And.Subject.First()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<TView>();
    }

    public static TView GetContentDialogView<TView, TViewModel>(this Window window)
        where TView : UserControl
        where TViewModel : ViewModelBase
    {
        return window
            .GetErrorDialogHost()
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ProgressDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("InputDialogHost"))
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ContentDialogHost"))
            .Case(dh => dh.IsOpen.Should().Be(true))
            .Case(dh =>
                dh.DialogContent.ThrowIfNull().Case(dc => dc.ThrowIfIsNotCast<TViewModel>())
            )
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Children.TakeLast(1)
            .Single()
            .ThrowIfIsNotCast<DialogOverlayPopupHost>()
            .GetVisualChildren()
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
            .Case(dh =>
                dh.DialogContent.ThrowIfNull().Case(dc => dc.ThrowIfIsNotCast<TViewModel>())
            )
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Children.TakeLast(1)
            .Single()
            .ThrowIfIsNotCast<DialogOverlayPopupHost>()
            .GetVisualChildren()
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
