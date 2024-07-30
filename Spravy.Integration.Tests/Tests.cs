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

        WindowHelper
            .CreateWindow()
            .TryCatch(
                w =>
                    w.SetSize(1000, 1000)
                        .ShowWindow()
                        .Case(
                            () =>
                                w.Case(
                                        () =>
                                            w.GetCurrentView<LoginView, LoginViewModel>()
                                                .RunJobsAll(1)
                                                .Case(l =>
                                                    l.GetControl<TextBox>(ElementNames.LoginTextBox)
                                                        .SetText(w, TextHelper.TextLength4)
                                                )
                                                .Case(
                                                    () =>
                                                        w.KeyHandleQwerty(
                                                            PhysicalKey.Enter,
                                                            RawInputModifiers.None
                                                        )
                                                )
                                                .RunJobsAll(1)
                                                .Case(l =>
                                                    l.GetControl<TextBox>(
                                                            ElementNames.PasswordTextBox
                                                        )
                                                        .SetText(w, TextHelper.TextLength8, false)
                                                )
                                                .Case(
                                                    () =>
                                                        w.KeyHandleQwerty(
                                                            PhysicalKey.Enter,
                                                            RawInputModifiers.None
                                                        )
                                                )
                                                .RunJobsAll(5)
                                                .Case(
                                                    () =>
                                                        w.GetErrorDialogView<
                                                            InfoView,
                                                            InfoViewModel
                                                        >()
                                                            .Case(i =>
                                                                i.FindControl<ContentControl>(
                                                                        ElementNames.ContentContentControl
                                                                    )
                                                                    .ThrowIfNull()
                                                                    .GetContentView<ErrorView>()
                                                                    .FindControl<ItemsControl>(
                                                                        ElementNames.ErrorsItemsControl
                                                                    )
                                                                    .ThrowIfNull()
                                                                    .Case(ic =>
                                                                        ic.ItemCount.Should().Be(1)
                                                                    )
                                                                    .GetVisualChildren()
                                                                    .Single()
                                                                    .ThrowIfIsNotCast<Border>()
                                                                    .Child.ThrowIfNull()
                                                                    .GetVisualChildren()
                                                                    .Single()
                                                                    .ThrowIfIsNotCast<StackPanel>()
                                                                    .Children.Single()
                                                                    .ThrowIfIsNotCast<ContentPresenter>()
                                                                    .Child.ThrowIfNull()
                                                                    .ThrowIfIsNotCast<StackPanel>()
                                                                    .Children.ElementAt(1)
                                                                    .ThrowIfIsNotCast<TextBlock>()
                                                                    .Text.Should()
                                                                    .Be(
                                                                        $"User with login \"{TextHelper.TextLength4}\" not exists"
                                                                    )
                                                            )
                                                            .FindControl<Button>(
                                                                ElementNames.OkButton
                                                            )
                                                            .ThrowIfNull()
                                                            .ClickOn(w)
                                                )
                                                .Case(l =>
                                                    l.FindControl<TextBox>(
                                                            ElementNames.LoginTextBox
                                                        )
                                                        .ThrowIfNull()
                                                        .FocusInput(w)
                                                        .ClearText(w)
                                                )
                                                .FindControl<Button>(ElementNames.CreateUserButton)
                                                .ThrowIfNull()
                                                .ClickOn(w)
                                                .RunJobsAll(1)
                                    )
                                    .Case(
                                        () =>
                                            w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                                                .FindControl<Button>(ElementNames.BackButton)
                                                .ThrowIfNull()
                                                .ClickOn(w)
                                    )
                                    .RunJobsAll(1)
                                    .Case(
                                        () =>
                                            w.GetCurrentView<LoginView, LoginViewModel>()
                                                .FindControl<Button>(ElementNames.CreateUserButton)
                                                .ThrowIfNull()
                                                .ClickOn(w)
                                    )
                                    .GetCurrentView<CreateUserView, CreateUserViewModel>()
                                    .Case(c =>
                                        c.FindControl<TextBox>(ElementNames.EmailTextBox)
                                            .ThrowIfNull()
                                            .MustFocused()
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength4
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength3
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength51
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength50
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.EmailLength51
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.EmailLength50
                                            )
                                            .Case(() => w.SetKeyTextInput(TextHelper.Email))
                                    )
                                    .Case(
                                        () =>
                                            w.KeyHandleQwerty(
                                                PhysicalKey.Enter,
                                                RawInputModifiers.None
                                            )
                                    )
                                    .RunJobsAll(1)
                                    .Case(c =>
                                        c.FindControl<TextBox>(ElementNames.LoginTextBox)
                                            .ThrowIfNull()
                                            .MustFocused()
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength513
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextWithSpaceLength512
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength3
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextWithSpaceLength8
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextLength512
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextLength4
                                            )
                                            .Case(() => w.SetKeyTextInput(TextHelper.TextLength4))
                                    )
                                    .Case(
                                        () =>
                                            w.KeyHandleQwerty(
                                                PhysicalKey.Enter,
                                                RawInputModifiers.None
                                            )
                                    )
                                    .RunJobsAll(1)
                                    .Case(c =>
                                        c.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                            .ThrowIfNull()
                                            .MustFocused()
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength513
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength7
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextWithSpaceLength512
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextWithSpaceLength8
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextLength512
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextLength8
                                            )
                                            .Case(() => w.SetKeyTextInput(TextHelper.TextLength8))
                                    )
                                    .Case(
                                        () =>
                                            w.KeyHandleQwerty(
                                                PhysicalKey.Enter,
                                                RawInputModifiers.None
                                            )
                                    )
                                    .RunJobsAll(1)
                                    .Case(c =>
                                        c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
                                            .ThrowIfNull()
                                            .MustFocused()
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength513
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength7
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextWithSpaceLength512
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextWithSpaceLength8
                                            )
                                            .ValidateCreateUserViewTextBoxError(
                                                w,
                                                c,
                                                TextHelper.TextLength512
                                            )
                                            .ValidateCreateUserViewTextBox(
                                                w,
                                                c,
                                                TextHelper.TextLength8
                                            )
                                            .Case(() => w.SetKeyTextInput(TextHelper.TextLength512))
                                    )
                                    .Case(
                                        () =>
                                            w.KeyHandleQwerty(
                                                PhysicalKey.Enter,
                                                RawInputModifiers.None
                                            )
                                    )
                        )
                        .Case(() => TestAppBuilder.Configuration.GetImapConnection().ClearInbox())
                        .Case(
                            () =>
                                w.GetCurrentView<CreateUserView, CreateUserViewModel>()
                                    .Case(c =>
                                        c.FindControl<TextBox>(ElementNames.RepeatPasswordTextBox)
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
                        .Case(
                            () =>
                                w.GetCurrentView<VerificationCodeView, VerificationCodeViewModel>()
                                    .FindControl<TextBox>(ElementNames.VerificationCodeTextBox)
                                    .ThrowIfNull()
                                    .FocusInput(w)
                                    .Case(
                                        () =>
                                            w.SetKeyTextInput(
                                                TestAppBuilder
                                                    .Configuration.GetImapConnection()
                                                    .GetLastEmailText()
                                            )
                                    )
                                    .FindControl<Button>(ElementNames.VerificationEmailButton)
                                    .ThrowIfNull()
                                    .MustEnabled()
                                    .ClickOn(w)
                                    .RunJobsAll(3)
                        )
                        .Case(
                            () =>
                                w.GetCurrentView<LoginView, LoginViewModel>()
                                    .Case(view =>
                                        view.FindControl<TextBox>(ElementNames.LoginTextBox)
                                            .ThrowIfNull()
                                            .SetText(w, TextHelper.TextLength4)
                                    )
                                    .Case(view =>
                                        view.FindControl<TextBox>(ElementNames.PasswordTextBox)
                                            .ThrowIfNull()
                                            .SetText(w, TextHelper.TextLength8)
                                    )
                                    .FindControl<Button>(ElementNames.LoginButton)
                                    .ThrowIfNull()
                                    .ClickOn(w)
                                    .RunJobsAll(3)
                        )
                        .Case(() => w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>())
                        .Close(),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest]
    [Order(1)]
    [Property("Priority", "1")]
    public void ForgotPasswordFlow()
    {
        LogCurrentTestMethod();

        WindowHelper
            .CreateWindow()
            .TryCatch(
                w =>
                    w.SetSize(1000, 1000)
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
                                w.GetCurrentView<
                                    EmailOrLoginInputView,
                                    EmailOrLoginInputViewModel
                                >()
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
                                w.GetCurrentView<
                                    EmailOrLoginInputView,
                                    EmailOrLoginInputViewModel
                                >()
                                    .Case(view =>
                                        view.FindControl<TextBox>("EmailOrLoginTextBox")
                                            .ThrowIfNull()
                                            .SetText(w, TextHelper.Email)
                                    )
                                    .FindControl<Button>("ForgotPasswordButton")
                                    .ThrowIfNull()
                                    .ClickOn(w)
                                    .RunJobsAll(5)
                        )
                        .Case(
                            () =>
                                TestAppBuilder.Configuration.GetImapConnection().GetLastEmailText()
                        ),
                (w, _) => w.SaveFrame().LogCurrentState()
            );
    }

    [AvaloniaTest]
    [Order(2)]
    [Property("Priority", "2")]
    public void TestAddToDoItemFlow()
    {
        LogCurrentTestMethod();

        WindowHelper
            .CreateWindow()
            .TryCatch(
                w =>
                    w.SetSize(1000, 1000)
                        .ShowWindow()
                        .RunJobsAll(6)
                        .Case(
                            () =>
                                w.GetCurrentView<RootToDoItemsView, RootToDoItemsViewModel>()
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

        WindowHelper
            .CreateWindow()
            .TryCatch(
                w =>
                    w.SetSize(1000, 1000)
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
                                    .ItemCount.Should()
                                    .Be(1)
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

        WindowHelper
            .CreateWindow()
            .TryCatch(
                w =>
                    w.SetSize(1000, 1000)
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
                                    .Case(c =>
                                        c.GetControl<ContentControl>(
                                                ElementNames.ContentContentControl
                                            )
                                            .GetContentView<ResetToDoItemView>()
                                            .GetControl<CheckBox>(
                                                ElementNames.IsMoveCircleOrderIndexCheckBoxName
                                            )
                                            .Case(cc => cc.IsChecked.Should().Be(true))
                                            .ClickOn(w)
                                            .RunJobsAll(1)
                                            .Case(cc => cc.IsChecked = false)
                                            .IsChecked.Should()
                                            .Be(false)
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
                            () =>
                                w.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                                    .Case(c =>
                                        c.FindControl<ContentControl>(
                                                ElementNames.ContentContentControl
                                            )
                                            .ThrowIfNull()
                                            .GetContentView<ResetToDoItemView>()
                                            .GetControl<CheckBox>(
                                                ElementNames.IsMoveCircleOrderIndexCheckBoxName
                                            )
                                            .IsChecked.Should()
                                            .Be(false)
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
