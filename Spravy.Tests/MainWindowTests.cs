using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Spravy.Tests.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;
using Xunit.Abstractions;

namespace Spravy.Tests;

public class MainWindowTests
{
    private readonly ITestOutputHelper output;

    public MainWindowTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [AvaloniaFact]
    public void CreateUserFlow()
    {
        return;
        output.WriteLine($"Start test: {nameof(CreateUserFlow)}");

        WindowHelper.CreateWindow(output)
            .TryCatch(
                w => w.SetSize(1000, 1000, output)
                    .ShowWindow(output)
                    .Case(
                        () => w.Case(
                                () => w.GetCurrentView<LoginView, LoginViewModel>()
                                    .FindControl<Button>(ElementNames.CreateUserButton)
                                    .ThrowIfNull()
                                    .ClickOnButton(w, output)
                            )
                            .Case(
                                () => w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                                    .FindControl<Button>(ElementNames.BackButton)
                                    .ThrowIfNull()
                                    .ClickOnButton(w, output)
                            )
                            .Case(
                                () => w.GetCurrentView<LoginView, LoginViewModel>()
                                    .FindControl<Button>(ElementNames.CreateUserButton)
                                    .ThrowIfNull()
                                    .ClickOnButton(w, output)
                            )
                            .GetCurrentView<CreateUserView, CreateUserViewModel>()
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.EmailTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength4, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength3, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength51, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength50, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.EmailLength51, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.EmailLength50, output)
                                    .Case(() => w.SetKeyTextInput(TextHelper.Email, output))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None, output))
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.LoginTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength513, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength512, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength3, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength8, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength512, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength4, output)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength4, output))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None, output))
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength513, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength7, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextWithSpaceLength512, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextWithSpaceLength8, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength512, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength8, output)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength8, output))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None, output))
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength513, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength7, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength512, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextWithSpaceLength8, output)
                                    .ValidateCreateUserViewTextBoxError(w, c, TextHelper.TextLength512, output)
                                    .ValidateCreateUserViewTextBox(w, c, TextHelper.TextLength8, output)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength512, output))
                            )
                            .Case(() => w.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None, output))
                    )
                    .Case(
                        () => w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                                    .ThrowIfNull()
                                    .ClearText(w, output)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength8, output))
                            )
                            .FindControl<Button>(ElementNames.CreateUserButton)
                            .ThrowIfNull()
                            .MustEnabled()
                            .ClickOnButton(w, output)
                            .WhenAllTasksAsync(output)
                            .GetAwaiter()
                            .GetResult()
                    )
                    .Case(() => w.GetCurrentView<VerificationCodeView, VerificationCodeViewModel>())
                    .SaveFrame(output),
                (w, _) => w.SaveFrame(output)
            );

        output.WriteLine($"End test: {nameof(CreateUserFlow)}");
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