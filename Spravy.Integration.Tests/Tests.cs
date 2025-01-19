using Shouldly;

namespace Spravy.Integration.Tests;

[TestFixture]
public class Tests
{
    static Tests()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [AvaloniaTest]
    [Order(0)]
    [Property("Priority", "0")]
    public void CreateUserFlow()
    {
        LogCurrentTestMethod();
        Directory.GetCurrentDirectory().ToDirectory().Combine("storage").DeleteIfExists(true);
        var window = WindowHelper.CreateWindow();

        try
        {
            window.SetSize(1000, 1000)
               .ShowWindow();

            window.WaitUntil(window.GetCurrentView<LoginView, LoginViewModel>)
               .Case(
                    l =>
                        l.GetControl<TextBox>(ElementNames.LoginTextBox)
                           .SetText(window, TextHelper.TextLength4)
                )
               .Case(() => window.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
               .RunJobsAll(1)
               .Case(
                    l =>
                        l.GetControl<TextBox>(ElementNames.PasswordTextBox)
                           .SetText(window, TextHelper.TextLength8, false)
                )
               .Case(() => window.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
               .Case(
                    () =>
                        window.WaitUntil(window.GetErrorDialogView<InfoView, InfoViewModel>)
                           .Case(
                                i =>
                                {
                                    var errorsItemsControl = i.FindControl<
                                            ContentControl>(
                                            ElementNames.ContentContentControl
                                        )
                                       .ThrowIfNull()
                                       .GetContentView<ErrorView>()
                                       .FindControl<
                                            ItemsControl>(ElementNames.ErrorsItemsControl)
                                       .ThrowIfNull();

                                    errorsItemsControl.ItemCount.ShouldBe(1);

                                    var text = errorsItemsControl.GetVisualChildren()
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
                                       .ThrowIfNull();

                                    text.ShouldBe(
                                        $"User with login \"{TextHelper.TextLength4}\" not exists"
                                    );
                                }
                            )
                           .FindControl<Button>(ElementNames.OkButton)
                           .ThrowIfNull()
                           .ClickOn(window)
                )
               .Case(
                    l =>
                        l.FindControl<TextBox>(ElementNames.LoginTextBox)
                           .ThrowIfNull()
                           .FocusInput(window)
                           .ClearText(window)
                )
               .FindControl<Button>(ElementNames.CreateUserButton)
               .ThrowIfNull()
               .ClickOn(window)
               .RunJobsAll(1);

            window.GetCurrentView<CreateUserView, CreateUserViewModel>()
               .FindControl<Button>(ElementNames.BackButton)
               .ThrowIfNull()
               .ClickOn(window)
               .RunJobsAll(1);

            window.GetCurrentView<LoginView, LoginViewModel>()
               .FindControl<Button>(ElementNames.CreateUserButton)
               .ThrowIfNull()
               .ClickOn(window);

            window
               .GetCurrentView<CreateUserView, CreateUserViewModel>()
               .Case(
                    c =>
                        c.FindControl<TextBox>(ElementNames.EmailTextBox)
                           .ThrowIfNull()
                           .MustFocused()
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength4)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength3)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength51)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength50)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.EmailLength51)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.EmailLength50)
                           .Case(() => window.SetKeyTextInput(TextHelper.Email))
                )
               .Case(() => window.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
               .RunJobsAll(1)
               .Case(
                    c =>
                        c.FindControl<TextBox>(ElementNames.LoginTextBox)
                           .ThrowIfNull()
                           .MustFocused()
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength513)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextWithSpaceLength512)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength3)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextWithSpaceLength8)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextLength512)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextLength4)
                           .Case(() => window.SetKeyTextInput(TextHelper.TextLength4))
                )
               .Case(() => window.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
               .RunJobsAll(1)
               .Case(
                    c =>
                        c.FindControl<TextBox>(ElementNames.PasswordTextBox)
                           .ThrowIfNull()
                           .MustFocused()
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength513)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength7)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextWithSpaceLength512)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextWithSpaceLength8)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextLength512)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextLength8)
                           .Case(() => window.SetKeyTextInput(TextHelper.TextLength8))
                )
               .Case(() => window.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
               .RunJobsAll(1)
               .Case(
                    c =>
                        c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                           .ThrowIfNull()
                           .MustFocused()
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength513)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength7)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextWithSpaceLength512)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextWithSpaceLength8)
                           .ValidateCreateUserViewTextBoxError(window, c, TextHelper.TextLength512)
                           .ValidateCreateUserViewTextBox(window, c, TextHelper.TextLength8)
                           .Case(() => window.SetKeyTextInput(TextHelper.TextLength512))
                )
               .Case(() => window.KeyHandleQwerty(PhysicalKey.Enter, RawInputModifiers.None))
               .Case(() => TestAppBuilder.Configuration.GetImapConnection().ClearInbox())
               .Case(
                    () =>
                        window.GetCurrentView<CreateUserView, CreateUserViewModel>()
                           .Case(
                                c =>
                                    c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                                       .ThrowIfNull()
                                       .ClearText(window)
                                       .Case(() => window.SetKeyTextInput(TextHelper.TextLength8))
                            )
                           .FindControl<Button>(ElementNames.CreateUserButton)
                           .ThrowIfNull()
                           .MustEnabled()
                           .ClickOn(window)
                           .RunJobsAll(2)
                )
               .Case(
                    () =>
                        window.GetCurrentView<VerificationCodeView, VerificationCodeViewModel>()
                           .FindControl<TextBox>(ElementNames.VerificationCodeTextBox)
                           .ThrowIfNull()
                           .FocusInput(window)
                           .Case(
                                () =>
                                    window.SetKeyTextInput(
                                        TestAppBuilder.Configuration.GetImapConnection().GetLastEmailText()
                                    )
                            )
                           .FindControl<Button>(ElementNames.VerificationEmailButton)
                           .ThrowIfNull()
                           .MustEnabled()
                           .ClickOn(window)
                           .RunJobsAll(3)
                )
               .Case(
                    () => window.GetCurrentView<LoginView, LoginViewModel>()
                       .Case(
                            view =>
                                view.FindControl<TextBox>(ElementNames.LoginTextBox)
                                   .ThrowIfNull()
                                   .SetText(window, TextHelper.TextLength4)
                        )
                       .Case(
                            view =>
                                view.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                   .ThrowIfNull()
                                   .SetText(window, TextHelper.TextLength8)
                        )
                       .FindControl<Button>(ElementNames.LoginButton)
                       .ThrowIfNull()
                       .ClickOn(window)
                       .RunJobsAll(11)
                )
               .Case(() => window.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>());

            window.Close();
        }
        catch
        {
            window.SaveFrame().LogCurrentState();

            throw;
        }
    }

    [AvaloniaTest]
    [Order(1)]
    [Property("Priority", "1")]
    public void ForgotPasswordFlow()
    {
        LogCurrentTestMethod();

        WindowHelper.CreateWindow()
           .TryCatch(
                w => w.SetSize(1000, 1000)
                   .ShowWindow()
                   .Case(
                        () =>
                            w.GetCurrentView<LoginView, LoginViewModel>()
                               .RunJobsAll(1)
                               .FindControl<Button>(ElementNames.ForgotPasswordButton)
                               .ThrowIfNull()
                               .ClickOn(w)
                               .RunJobsAll(1)
                    )
                   .Case(
                        () =>
                            w.GetCurrentView<EmailOrLoginInputView, EmailOrLoginInputViewModel>()
                               .FindControl<Button>("BackButton")
                               .ThrowIfNull()
                               .ClickOn(w)
                               .RunJobsAll(1)
                    )
                   .Case(
                        () =>
                            w.GetCurrentView<LoginView, LoginViewModel>()
                               .FindControl<Button>(ElementNames.ForgotPasswordButton)
                               .ThrowIfNull()
                               .ClickOn(w)
                               .RunJobsAll(1)
                    )
                   .Case(() => TestAppBuilder.Configuration.GetImapConnection().ClearInbox())
                   .Case(
                        () =>
                            w.GetCurrentView<EmailOrLoginInputView, EmailOrLoginInputViewModel>()
                               .Case(
                                    view =>
                                        view.FindControl<TextBox>("EmailOrLoginTextBox")
                                           .ThrowIfNull()
                                           .SetText(w, TextHelper.Email)
                                )
                               .FindControl<Button>("ForgotPasswordButton")
                               .ThrowIfNull()
                               .ClickOn(w)
                               .RunJobsAll(12)
                    )
                   .Case(
                        () =>
                            w.GetCurrentView<ForgotPasswordView, ForgotPasswordViewModel>()
                               .Case(
                                    view =>
                                        view.FindControl<TextBox>("VerificationCodeTextBox")
                                           .ThrowIfNull()
                                           .SetText(
                                                w,
                                                TestAppBuilder.Configuration.GetImapConnection().GetLastEmailText()
                                            )
                                )
                               .Case(
                                    view =>
                                        view.FindControl<TextBox>("NewPasswordTextBox")
                                           .ThrowIfNull()
                                           .SetText(w, TextHelper.TextLength50)
                                )
                               .Case(
                                    view =>
                                        view.FindControl<TextBox>("NewRepeatPasswordTextBox")
                                           .ThrowIfNull()
                                           .SetText(w, TextHelper.TextLength50)
                                )
                               .FindControl<Button>("ForgotPasswordButton")
                               .ThrowIfNull()
                               .ClickOn(w)
                               .RunJobsAll(3)
                    )
                   .Case(
                        () => w.GetCurrentView<LoginView, LoginViewModel>()
                           .Case(
                                view =>
                                    view.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                       .ThrowIfNull()
                                       .SetText(w, TextHelper.TextLength8)
                            )
                           .Case(
                                view =>
                                    view.FindControl<CheckBox>(ElementNames.RememberMeCheckBoxName)
                                       .ThrowIfNull()
                                       .ClickOn(w)
                            )
                           .FindControl<Button>(ElementNames.LoginButton)
                           .ThrowIfNull()
                           .ClickOn(w)
                           .RunJobsAll(4)
                    )
                   .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest]
    [Order(2)]
    [Property("Priority", "2")]
    public void TestAddToDoItemFlow()
    {
        LogCurrentTestMethod();

        WindowHelper.CreateWindow()
           .TryCatch(
                w => w.SetSize(1000, 1000)
                   .ShowWindow()
                   .RunJobsAll(6)
                   .Case(
                        () => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                           .AddToDoItem(w, "To-Do item 1")
                           .AddToDoItem(w, "To-Do item 2")
                    )
                   .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest]
    [Order(3)]
    [Property("Priority", "3")]
    public void TestChangeToDoItemFlow()
    {
        LogCurrentTestMethod();

        WindowHelper.CreateWindow()
           .TryCatch(
                w => w.SetSize(1000, 1000)
                   .ShowWindow()
                   .RunJobsAll(6)
                   .Case(
                        () =>
                            w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                               .Case(r => r.GetToDoItemDotsButton(0).ClickOn(w).RunJobsAll(1))
                               .GetToDoItemReorder(0)
                               .ClickOn(w)
                               .RunJobsAll(2)
                    )
                   .Case(
                        () =>
                            w.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                               .FindControl<ContentControl>(ElementNames.ContentContentControl)
                               .ThrowIfNull()
                               .GetContentView<ChangeToDoItemOrderIndexView>()
                               .GetControl<ListBox>(ElementNames.ItemsListBox)
                               .ItemCount
                               .ShouldBe(1)
                    )
                   .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest]
    [Order(4)]
    [Property("Priority", "4")]
    public void TestResetToDoItemFlow()
    {
        LogCurrentTestMethod();

        WindowHelper.CreateWindow()
           .TryCatch(
                w => w.SetSize(1000, 1000)
                   .ShowWindow()
                   .RunJobsAll(5)
                   .Case(
                        () =>
                            w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                               .Case(r => r.GetToDoItemDotsButton(0).ClickOn(w).RunJobsAll(1))
                               .GetToDoItemReset(0)
                               .ClickOn(w)
                               .RunJobsAll(2)
                    )
                   .Case(
                        () =>
                            w.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                               .Case(
                                    c =>
                                    {
                                        var checkBox = c.GetControl<ContentControl>(ElementNames.ContentContentControl)
                                           .GetContentView<ResetToDoItemView>()
                                           .GetControl<CheckBox>(ElementNames.IsMoveCircleOrderIndexCheckBoxName);

                                        checkBox.IsChecked.ShouldBe(true);

                                        checkBox.ClickOn(w)
                                           .RunJobsAll(1);

                                        checkBox.IsChecked.ShouldBe(false);
                                    }
                                )
                               .GetControl<Button>(ElementNames.OkButton)
                               .ClickOn(w)
                               .RunJobsAll(1)
                    )
                   .Case(
                        () =>
                            w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
                               .Case(r => r.GetToDoItemDotsButton(0).ClickOn(w).RunJobsAll(1))
                               .GetToDoItemReset(0)
                               .ClickOn(w)
                               .RunJobsAll(2)
                    )
                   .Case(
                        () => w.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                           .Case(
                                c =>
                                    c.FindControl<ContentControl>(ElementNames.ContentContentControl)
                                       .ThrowIfNull()
                                       .GetContentView<ResetToDoItemView>()
                                       .GetControl<CheckBox>(ElementNames.IsMoveCircleOrderIndexCheckBoxName)
                                       .IsChecked
                                       .ShouldBe(false)
                            )
                    )
                   .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    private void LogCurrentTestMethod()
    {
        var methodName = TestContext.CurrentContext.Test.Name;
        Console.WriteLine($"Currently running test: {methodName}");
    }
}