using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.VisualTree;
using DialogHostAvalonia;
using FluentAssertions;
using Spravy.Domain.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Tests.Extensions;

public static class WindowExtension
{
    public static TWindow CloseErrorDialogHost<TWindow>(this TWindow window) where TWindow : Window
    {
        var dialogHost = window.GetErrorDialogHost();
        dialogHost.IsOpen.Should().BeTrue();
        Thread.Sleep(TimeSpan.FromSeconds(1));

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
        
        Thread.Sleep(TimeSpan.FromSeconds(1));
        dialogHost.IsOpen.Should().BeFalse();

        return window;
    }

    public static TWindow MustShowError<TWindow>(this TWindow window) where TWindow : Window
    {
        window.GetErrorDialogHost().IsOpen.Should().BeTrue();

        return window;
    }

    public static TWindow SetKeyTextInput<TWindow>(this TWindow window, string text) where TWindow : Window
    {
        window.KeyTextInput(text);
        window.RunJobsAll();

        return window;
    }

    public static TWindow KeyHandleQwerty<TWindow>(
        this TWindow window,
        PhysicalKey key,
        RawInputModifiers modifiers = RawInputModifiers.None
    )
        where TWindow : Window
    {
        window.KeyPressQwerty(key, modifiers);
        window.RunJobsAll();
        window.KeyReleaseQwerty(key, modifiers);
        window.RunJobsAll();

        return window;
    }

    public static TWindow SetSize<TWindow>(this TWindow window, double width, double height) where TWindow : Window
    {
        window.Width = width;
        window.Height = height;
        window.RunJobsAll();

        return window;
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window) where TWindow : Window
    {
        return window.RunJobsAll().SaveFrame(FileHelper.GetFrameShortFile());
    }

    public static TWindow SaveFrame<TWindow>(this TWindow window, FileInfo file) where TWindow : Window
    {
        using var bitmap = window.CaptureRenderedFrame().ThrowIfNull();
        bitmap.Save(file.FullName);

        return window;
    }

    public static CreateUserView NavigateToCreateUserView<TWindow>(this TWindow window) where TWindow : Window
    {
        return window.Case(
                w => w.GetCurrentView<LoginView, LoginViewModel>()
                    .FindControl<Button>("CreateUserButton")
                    .ThrowIfNull()
                    .ClickOnButton(w)
            )
            .GetCurrentView<CreateUserView, CreateUserViewModel>();
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
            .Case(w => w.DataContext.ThrowIfNull().ThrowIfIsNotCast<MainWindowModel>())
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