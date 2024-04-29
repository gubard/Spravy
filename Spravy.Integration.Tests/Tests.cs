using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using FluentAssertions;
using Spravy.Domain.Extensions;
using Spravy.Integration.Tests.Extensions;
using Spravy.Integration.Tests.Helpers;
using Spravy.Ui.Features.Authentication.ViewModels;
using Spravy.Ui.Features.Authentication.Views;
using Spravy.Ui.Features.ErrorHandling.Views;
using Spravy.Ui.Features.ToDo.ViewModels;
using Spravy.Ui.Features.ToDo.Views;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;
using SukiUI.Controls;

namespace Spravy.Integration.Tests;

[TestFixture]
public class Tests
{
    static Tests()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [AvaloniaTest, Order(0), Property("Priority", "0")]
    public void CreateUserFlow()
    {
        WindowHelper.CreateWindow()
            .TryCatch(
                w => w.SetSize(1000, 1000)
                    .ShowWindow()
                    .Case(
                        () => w.Case(
                                () => w.GetCurrentView<LoginView, LoginViewModel>()
                                    .RunJobsAll(1)
                                    .Case(l => l.FindControl<TextBox>(ElementNames.LoginTextBox)
                                        .ThrowIfNull()
                                        .SetText(w, TextHelper.TextLength4))
                                    .Case(l => l.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                        .ThrowIfNull()
                                        .SetText(w, TextHelper.TextLength8))
                                    .Case(l => l.FindControl<Button>(ElementNames.LoginButton)
                                        .ThrowIfNull()
                                        .ClickOn(w)
                                        .RunJobsAll(5))
                                    .Case(() => w.GetErrorDialogView<InfoView, InfoViewModel>()
                                        .Case(i => i.FindControl<ContentControl>(ElementNames.ContentContentControl)
                                            .ThrowIfNull()
                                            .GetContentView<ErrorView>()
                                            .FindControl<ItemsControl>(ElementNames.ErrorsItemsControl)
                                            .ThrowIfNull()
                                            .Case(ic => ic.ItemCount.Should().Be(1))
                                            .GetVisualChildren()
                                            .Single()
                                            .ThrowIfIsNotCast<Border>()
                                            .Child
                                            .ThrowIfNull()
                                            .GetVisualChildren()
                                            .Single()
                                            .ThrowIfIsNotCast<StackPanel>()
                                            .Children
                                            .Single()
                                            .ThrowIfIsNotCast<ContentPresenter>()
                                            .Child
                                            .ThrowIfNull()
                                            .ThrowIfIsNotCast<StackPanel>()
                                            .Children
                                            .ElementAt(1)
                                            .ThrowIfIsNotCast<TextBlock>()
                                            .Text
                                            .Should()
                                            .Be($"User with login \"{TextHelper.TextLength4}\" not exists"))
                                        .FindControl<Button>(ElementNames.OkButton)
                                        .ThrowIfNull()
                                        .ClickOn(w))
                                    .Case(l => l.FindControl<TextBox>(ElementNames.LoginTextBox)
                                        .ThrowIfNull()
                                        .FocusInput(w)
                                        .ClearText(w))
                                    .FindControl<Button>(ElementNames.CreateUserButton)
                                    .ThrowIfNull()
                                    .ClickOn(w)
                                    .RunJobsAll(1)
                            )
                            .Case(
                                () => w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                                    .FindControl<Button>(ElementNames.BackButton)
                                    .ThrowIfNull()
                                    .ClickOn(w)
                            )
                            .Case(
                                () => w.GetCurrentView<LoginView, LoginViewModel>()
                                    .FindControl<Button>(ElementNames.CreateUserButton)
                                    .ThrowIfNull()
                                    .ClickOn(w)
                            )
                            .GetCurrentView<CreateUserView, CreateUserViewModel>()
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.EmailTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength4)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength3)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength51)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength50)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.EmailLength51)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.EmailLength50)
                                    .Case(() => w.SetKeyTextInput(TextHelper.Email))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.LoginTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength513)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength512)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength3)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength8)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength512)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength4)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength4))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength513)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength7)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextWithSpaceLength512)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextWithSpaceLength8)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength512)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength8)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength8))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength513)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength7)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength512)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength8)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength512)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength8)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength512))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
                    )
                    .Case(() => TestAppBuilder.Configuration.GetImapConnection().ClearInbox())
                    .Case(
                        () => w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                                    .ThrowIfNull()
                                    .ClearText(w)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength8))
                            )
                            .FindControl<Button>(ElementNames.CreateUserButton)
                            .ThrowIfNull()
                            .MustEnabled()
                            .ClickOn(w)
                            .RunJobsAll(2)
                    )
                    .Case(() => w.GetCurrentView<VerificationCodeView, VerificationCodeViewModel>()
                        .FindControl<TextBox>(ElementNames.VerificationCodeTextBox)
                        .ThrowIfNull()
                        .FocusInput(w)
                        .Case(() => w.SetKeyTextInput(
                                TestAppBuilder.Configuration.GetImapConnection().GetLastEmailText()
                            )
                        )
                        .FindControl<Button>(ElementNames.VerificationEmailButton)
                        .ThrowIfNull()
                        .MustEnabled()
                        .ClickOn(w)
                        .RunJobsAll(3)
                    )
                    .Case(() => w.GetCurrentView<LoginView, LoginViewModel>()
                        .Case(view => view.FindControl<TextBox>(ElementNames.LoginTextBox)
                            .ThrowIfNull()
                            .SetText(w, TextHelper.TextLength4))
                        .Case(view => view.FindControl<TextBox>(ElementNames.PasswordTextBox)
                            .ThrowIfNull()
                            .SetText(w, TextHelper.TextLength8))
                        .Case(view => view.FindControl<CheckBox>(ElementNames.RememberMeCheckBoxName)
                            .ThrowIfNull()
                            .ClickOn(w))
                        .FindControl<Button>(ElementNames.LoginButton)
                        .ThrowIfNull()
                        .ClickOn(w)
                        .RunJobsAll(2))
                    .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>())
                    .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest, Order(1), Property("Priority", "1")]
    public void TestAddToDoItemFlow()
    {
        WindowHelper.CreateWindow()
            .TryCatch(
                w => w.SetSize(1000, 1000)
                    .ShowWindow()
                    .RunJobsAll(5)
                    .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                        .AddToDoItem(w, "To-Do item 1")
                        .AddToDoItem(w, "To-Do item 2"))
                    .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest, Order(2), Property("Priority", "2")]
    public void TestChangeToDoItemFlow()
    {
        WindowHelper.CreateWindow()
            .TryCatch(
                w => w.SetSize(1000, 1000)
                    .ShowWindow()
                    .RunJobsAll(5)
                    .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                        .GetDoItemItemsControl()
                        .ThrowIfNull()
                        .GetVisualChildren()
                        .Single()
                        .Case(a => a.ToString())
                        .ThrowIfIsNotCast<Border>()
                        .Child
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<ItemsPresenter>()
                        .GetVisualChildren()
                        .Single()
                        .ThrowIfIsNotCast<VirtualizingStackPanel>()
                        .Children
                        .Last()
                        .ThrowIfIsNotCast<ContentPresenter>()
                        .Child
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<BusyArea>()
                        .Content
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<Button>()
                        .Content
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<Grid>()
                        .Children
                        .Last()
                        .ThrowIfIsNotCast<Grid>()
                        .Children
                        .First()
                        .ThrowIfIsNotCast<Button>()
                        .ClickOn(w)
                        .RunJobsAll(1)
                        .Flyout
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<IPopupHostProvider>()
                        .PopupHost
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<Visual>()
                        .GetVisualChildren()
                        .Single()
                        .ThrowIfIsNotCast<LayoutTransformControl>()
                        .Child
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<VisualLayerManager>()
                        .Child
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<ContentPresenter>()
                        .Child
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<MenuFlyoutPresenter>()
                        .GetVisualChildren()
                        .Single()
                        .ThrowIfIsNotCast<Panel>()
                        .Children
                        .Last()
                        .ThrowIfIsNotCast<Border>()
                        .Child
                        .ThrowIfNull()
                        .ThrowIfIsNotCast<ItemsPresenter>()
                        .GetVisualChildren()
                        .Single()
                        .ThrowIfIsNotCast<StackPanel>()
                        .Children
                        .ElementAt(10)
                        .ClickOn(w)
                        .RunJobsAll(2))
                    .Case(() => w.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                        .FindControl<ContentControl>(ElementNames.ContentContentControl)
                        .ThrowIfNull()
                        .GetContentView<ChangeToDoItemOrderIndexView>()
                        .GetControl<ListBox>(ElementNames.ItemsListBox)
                        .ItemCount
                        .Should()
                        .Be(1))
                    .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }
}