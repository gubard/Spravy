using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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
                            .FocusInput(w))
                        .Case(() => w.SetKeyTextInput(TextHelper.TextLength4))
                        .Case(view => view.FindControl<TextBox>(ElementNames.PasswordTextBox)
                            .ThrowIfNull()
                            .FocusInput(w))
                        .Case(() => w.SetKeyTextInput(TextHelper.TextLength8))
                        .FindControl<Button>(ElementNames.LoginButton)
                        .ThrowIfNull()
                        .ClickOn(w)
                        .RunJobsAll(2))
                    .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>())
                    .SaveFrame()
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
                    .Case(() => w.GetCurrentView<LoginView, LoginViewModel>()
                        .RunJobsAll(1)
                        .Case(view => view.FindControl<TextBox>(ElementNames.LoginTextBox)
                            .ThrowIfNull()
                            .Text
                            .Should()
                            .Be(TextHelper.TextLength4))
                        .Case(view => view.FindControl<TextBox>(ElementNames.PasswordTextBox)
                            .ThrowIfNull()
                            .FocusInput(w))
                        .Case(() => w.SetKeyTextInput(TextHelper.TextLength8))
                        .FindControl<Button>(ElementNames.LoginButton)
                        .ThrowIfNull()
                        .ClickOn(w)
                        .RunJobsAll(4))
                    .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                        .FindControl<Button>(ElementNames.AddRootToDoItemButton)
                        .ThrowIfNull()
                        .ClickOn(w)
                        .RunJobsAll(2)
                        .Case(() => w.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                            .Case(c => c.FindControl<ContentControl>(ElementNames.ContentContentControl)
                                .ThrowIfNull()
                                .GetContentView<AddRootToDoItemView>()
                                .FindControl<ContentControl>(ElementNames.ToDoItemContentContentControl)
                                .ThrowIfNull()
                                .GetContentView<ToDoItemContentView>()
                                .FindControl<TextBox>(ElementNames.NameTextBox)
                                .ThrowIfNull()
                                .SetText(w, "To-Do item 1"))
                            .Case(c => c.FindControl<Button>(ElementNames.OkButton)
                                .ThrowIfNull()
                                .ClickOn(w)))
                        .Case(r => r.FindControl<ContentControl>(ElementNames.ToDoSubItemsContentControl)
                            .ThrowIfNull()
                            .GetContentView<ToDoSubItemsView>()
                            .FindControl<ContentControl>(ElementNames.ListContentControl)
                            .ThrowIfNull()
                            .GetContentView<MultiToDoItemsView>()
                            .FindControl<ContentControl>(ElementNames.ContentContentControl)
                            .ThrowIfNull()
                            .GetContentView<ToDoItemsGroupByView>()
                            .FindControl<ContentControl>(ElementNames.ContentContentControl)
                            .ThrowIfNull()
                            .GetContentView<ToDoItemsGroupByStatusView>()
                            .FindControl<ContentControl>(ElementNames.ReadyForCompletedContentControl)
                            .ThrowIfNull()
                            .GetContentView<ToDoItemsView>()
                            .FindControl<ItemsControl>(ElementNames.ItemsItemsControl)
                            .ThrowIfNull()
                            .Case(i => i.ItemCount.Should().Be(1))))
                    .SaveFrame()
                    .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }
}