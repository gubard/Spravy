using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.VisualTree;
using DialogHostAvalonia;
using FluentAssertions;
using Spravy.Domain.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;
using Xunit.Abstractions;

namespace Spravy.Tests.Extensions;

public static class WindowExtension
{
    public static TWindow CloseErrorDialogHost<TWindow>(this TWindow window, ITestOutputHelper output)
        where TWindow : Window
    {
        var dialogHost = window.GetErrorDialogHost();
        dialogHost.IsOpen.Should().BeTrue();

        dialogHost.GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Children
            .OfType<DialogOverlayPopupHost>()
            .Single()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Border>()
            .Child
            .ThrowIfNull()
            .ThrowIfIsNotCast<ContentPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Control>()
            .FindControl<Button>(ElementNames.OkButton)
            .ThrowIfNull()
            .ClickOnButton(window, output);

        dialogHost.IsOpen.Should().BeFalse();

        return window;
    }

    public static TWindow MustShowError<TWindow>(this TWindow window) where TWindow : Window
    {
        window.GetErrorDialogHost().IsOpen.Should().BeTrue();

        return window;
    }

    public static TWindow SetKeyTextInput<TWindow>(this TWindow window, string text, ITestOutputHelper output)
        where TWindow : Window
    {
        window.KeyTextInput(text, output);
        window.RunJobsAll(output);

        return window;
    }

    public static TWindow KeyHandleQwerty<TWindow>(
        this TWindow window,
        PhysicalKey key,
        RawInputModifiers modifiers,
        ITestOutputHelper output
    )
        where TWindow : Window
    {
        window.KeyPressQwerty(key, modifiers, output);
        window.RunJobsAll(output);
        window.KeyReleaseQwerty(key, modifiers, output);
        window.RunJobsAll(output);

        return window;
    }

    public static TWindow SetSize<TWindow>(this TWindow window, double width, double height, ITestOutputHelper output)
        where TWindow : Window
    {
        output.WriteLine($"Change window size from {window.Width}x{window.Height} to {width}x{height}");
        window.Width = width;
        window.Height = height;
        window.RunJobsAll(output);

        return window;
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window, ITestOutputHelper output) where TWindow : Window
    {
        return window.RunJobsAll(output).SaveFrame(FileHelper.GetFrameShortFile(), output);
    }

    public static async Task<TWindow> SaveFrameAsync<TWindow>(this Task<TWindow> task, ITestOutputHelper output)
        where TWindow : Window
    {
        var window = await task;

        return window.RunJobsAll(output).SaveFrame(FileHelper.GetFrameShortFile(), output);
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window, FileInfo file, ITestOutputHelper output)
        where TWindow : Window
    {
        output.WriteLine($"Capturing rendered frame");
        using var bitmap = window.CaptureRenderedFrame().ThrowIfNull();
        output.WriteLine($"Captured rendered frame");
        output.WriteLine($"Saving rendered frame to {file}");
        bitmap.Save(file.FullName);
        output.WriteLine($"Saved rendered frame to {file}");

        return window;
    }

    public static TWindow Show<TWindow>(this TWindow window, ITestOutputHelper output) where TWindow : Window
    {
        output.WriteLine($"Showing {window.Name}");
        window.Show();
        output.WriteLine($"Showed {window.Name}");

        return window;
    }

    public static TWindow ShowWindow<TWindow>(this TWindow window, ITestOutputHelper output) where TWindow : Window
    {
        window.Show(output);
        window.RunJobsAll(output);

        return window;
    }

    public static DialogHost GetErrorDialogHost(this Window window)
    {
        return window.ThrowIfIsNotCast<MainWindow>()
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<MainView>()
            .Case(w => w.DataContext.ThrowIfNull().ThrowIfIsNotCast<MainViewModel>())
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ErrorDialogHost"));
    }

    public static TView GetCurrentView<TView, TViewModel>(this Window window)
        where TView : UserControl where TViewModel : ViewModelBase
    {
        return window.GetErrorDialogHost()
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ProgressDialogHost"))
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("InputDialogHost"))
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<DialogHost>()
            .Case(dh => dh.Identifier.Should().Be("ContentDialogHost"))
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<ContentControl>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child
            .ThrowIfNull()
            .ThrowIfIsNotCast<MainSplitView>()
            .Case(w => w.DataContext.ThrowIfNull().ThrowIfIsNotCast<MainSplitViewModel>())
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<SplitView>()
            .Case(sv => sv.Pane.ThrowIfNull().ThrowIfIsNotCast<PaneViewModel>())
            .Case(sv => sv.Content.ThrowIfNull().ThrowIfIsNotCast<TViewModel>())
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Grid>()
            .Case(
                g => g.Children
                    .Should()
                    .HaveCount(2)
                    .And
                    .Subject
                    .First()
                    .ThrowIfIsNotCast<Panel>()
                    .Children
                    .Should()
                    .HaveCount(2)
                    .And
                    .Subject
                    .First()
                    .ThrowIfIsNotCast<ContentPresenter>()
                    .Child
                    .ThrowIfNull()
                    .ThrowIfIsNotCast<PaneView>()
            )
            .Children
            .Skip(1)
            .Single()
            .ThrowIfIsNotCast<Panel>()
            .Children
            .Should()
            .HaveCount(2)
            .And
            .Subject
            .First()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child
            .ThrowIfNull()
            .ThrowIfIsNotCast<TView>();
    }
}