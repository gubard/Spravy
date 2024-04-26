using System.Text;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Spravy.Tests.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.Features.Authentication.ViewModels;
using Spravy.Ui.Features.Authentication.Views;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Tests;

public class MainWindowTests
{
    static MainWindowTests()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [AvaloniaFact]
    public void CreateUserFlow()
    {
        WindowHelper.CreateWindow()
            .TryCatch(
                w => w.SetSize(1000, 1000)
                    .ShowWindow()
                    .Case(
                        () => w.Case(
                                () => w.GetCurrentView<LoginView, LoginViewModel>()
                                    .FindControl<Button>(ElementNames.CreateUserButton)
                                    .ThrowIfNull()
                                    .ClickOnButton(w)
                                    .RunJobsAll(5)
                            )
                            .Case(
                                () => w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                                    .FindControl<Button>(ElementNames.BackButton)
                                    .ThrowIfNull()
                                    .ClickOnButton(w)
                            )
                            .Case(
                                () => w.GetCurrentView<LoginView, LoginViewModel>()
                                    .FindControl<Button>(ElementNames.CreateUserButton)
                                    .ThrowIfNull()
                                    .ClickOnButton(w)
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
                            .ClickOnButton(w)
                            .RunJobsAll(2)
                    )
                    .Case(() => w.GetCurrentView<VerificationCodeView, VerificationCodeViewModel>()
                        .FindControl<TextBox>("VerificationCodeTextBox")
                        .ThrowIfNull()
                        .FocusElement()
                        .Case(() => w.SetKeyTextInput(
                                TestAppBuilder.Configuration.GetImapConnection().GetLastEmailText()
                            )
                        )
                        .FindControl<Button>("VerificationEmailButton")
                        .ThrowIfNull()
                        .MustEnabled()
                        .ClickOnButton(w)
                        .RunJobsAll(3)
                    )
                    .Case(() => w.GetCurrentView<LoginView, LoginViewModel>()
                        .Case(view => view.FindControl<TextBox>("LoginTextBox")
                            .ThrowIfNull()
                            .FocusElement())
                        .Case(() => w.SetKeyTextInput(TextHelper.TextLength4))
                        .Case(view => view.FindControl<TextBox>("PasswordTextBox")
                            .ThrowIfNull()
                            .FocusElement())
                        .Case(() => w.SetKeyTextInput(TextHelper.TextLength8))
                        .FindControl<Button>("LoginButton")
                        .ThrowIfNull()
                        .ClickOnButton(w))
                    .SaveFrame(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }
}