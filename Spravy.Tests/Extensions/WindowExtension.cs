using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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

        return window;
    }

    public static TView GetCurrentView<TView, TViewModel>(this Window window)
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
            .Case(dh => dh.Identifier.Should().Be("ErrorDialogHost"))
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