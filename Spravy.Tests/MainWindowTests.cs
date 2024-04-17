using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Spravy.Tests.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Tests;

public class MainWindowTests
{
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
                            .RunJobsAll(15)
                    )
                    .Case(() => w.GetCurrentView<VerificationCodeView, VerificationCodeViewModel>())
                    .SaveFrame(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );

        /*using var imapClient = new ImapClient(
                                   TestAppBuilder.Configuration.GetSection("EmailServer:Host").Value,
                                   TestAppBuilder.Configuration.GetSection("EmailAccount:Email").Value,
                                   TestAppBuilder.Configuration.GetSection("EmailAccount:Password").Value,
                                   AuthMethods.Login,
                                   993,
                                   true
                               );
                               using var pop = new Pop3Client(
                                   TestAppBuilder.Configuration.GetSection("EmailServer:Host").Value,
                                   TestAppBuilder.Configuration.GetSection("EmailAccount:Email").Value,
                                   TestAppBuilder.Configuration.GetSection("EmailAccount:Password").Value,
                                   995,
                                   true
                               );
                               var messages = imapClient.SearchMessages(SearchCondition.From(""));*/
    }
}