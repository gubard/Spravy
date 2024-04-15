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

namespace Spravy.Tests.Extensions;

public static class WindowExtension
{
    public static TWindow CloseErrorDialogHost<TWindow>(this TWindow window)
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
            .ClickOnButton(window);

        dialogHost.IsOpen.Should().BeFalse();

        return window;
    }

    public static TWindow MustShowError<TWindow>(this TWindow window) where TWindow : Window
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
        Console.WriteLine($"Change window size from {window.Width}x{window.Height} to {width}x{height}");
        window.Width = width;
        window.Height = height;
        window.RunJobsAll();

        return window;
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window) where TWindow : Window
    {
        return window.RunJobsAll().SaveFrame(FileHelper.GetFrameShortFile());
    }

    public static TWindow LogCurrentState<TWindow>(this TWindow window) where TWindow : Window
    {
        var mainContent = window.Find<ContentControl>("MainContent");
        var errorDialogHost = window.Find<DialogHost>("ErrorDialogHost");
        var progressDialogHost = window.Find<DialogHost>("ProgressDialogHost");
        var inputDialogHost = window.Find<DialogHost>("InputDialogHost");
        var contentDialogHost = window.Find<DialogHost>("ContentDialogHost");

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
        Console.WriteLine("Capturing rendered frame");
        using var bitmap = window.CaptureRenderedFrame().ThrowIfNull();
        Console.WriteLine("Captured rendered frame");
        Console.WriteLine($"Saving rendered frame to {file}");
        file.Directory.ThrowIfNull().Create();
        bitmap.Save(file.FullName);
        Console.WriteLine($"Saved rendered frame to {file}");

        return window;
    }

    public static TWindow Show<TWindow>(this TWindow window) where TWindow : Window
    {
        Console.WriteLine($"Showing {window.Name}");
        window.Show();
        Console.WriteLine($"Showed {window.Name}");

        return window;
    }

    public static TWindow ShowWindow<TWindow>(this TWindow window) where TWindow : Window
    {
        window.Show();
        window.RunJobsAll();

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