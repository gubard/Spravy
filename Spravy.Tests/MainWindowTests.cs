using System.Text;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
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
                                    .RunJobsAll(3)
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
                    .Case(() =>
                    {
                        using var client = new ImapClient();

                        try
                        {
                            Console.WriteLine(
                                $"{TestAppBuilder.Configuration.GetSection("EmailServer:Host").Value} {TestAppBuilder.Configuration.GetSection("EmailAccount:Email").Value} {TestAppBuilder.Configuration.GetSection("EmailAccount:Password").Value}");

                            client.Connect(
                                TestAppBuilder.Configuration.GetSection("EmailServer:Host").Value,
                                993,
                                SecureSocketOptions.SslOnConnect
                            );

                            client.Authenticate(
                                TestAppBuilder.Configuration.GetSection("EmailAccount:Email").Value,
                                TestAppBuilder.Configuration.GetSection("EmailAccount:Password").Value
                            );

                            var inbox = client.Inbox;
                            var i = 0;

                            while (true)
                            {
                                i++;
                                inbox.Open(FolderAccess.ReadWrite);

                                if (inbox.Count == 0)
                                {
                                    inbox.Close();

                                    return;
                                }

                                var trash = client.GetFolder("Trash");
                                inbox.MoveTo(0, trash);
                                inbox.Close();

                                if (i == 100)
                                {
                                    return;
                                }
                            }
                        }
                        finally
                        {
                            client.Disconnect(true);
                        }
                    })
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
                        .Case(tb =>
                        {
                            using var client = new ImapClient();

                            try
                            {
                                client.Connect(
                                    TestAppBuilder.Configuration.GetSection("EmailServer:Host").Value,
                                    993,
                                    SecureSocketOptions.SslOnConnect
                                );

                                client.Authenticate(
                                    TestAppBuilder.Configuration.GetSection("EmailAccount:Email").Value,
                                    TestAppBuilder.Configuration.GetSection("EmailAccount:Password").Value
                                );

                                var inbox = client.Inbox;
                                var i = 0;

                                while (true)
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    i++;
                                    inbox.Open(FolderAccess.ReadWrite);

                                    if (inbox.Count == 0)
                                    {
                                        inbox.Close();

                                        if (i == 100)
                                        {
                                            throw new Exception("Inbox timeout");
                                        }

                                        continue;
                                    }

                                    var message = inbox.GetMessage(0);
                                    tb.FocusElement();
                                    w.SetKeyTextInput(message.TextBody);

                                    return;
                                }
                            }
                            finally
                            {
                                client.Disconnect(true);
                            }
                        }))
                    .SaveFrame(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }
}