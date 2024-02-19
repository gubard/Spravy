using Avalonia.Controls;
using Spravy.Domain.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.Views;
using SukiUI.Controls;
using Xunit.Abstractions;

namespace Spravy.Tests.Extensions;

public static class CreateUserViewExtension
{
    public static TTextBox ValidateCreateUserViewTextBoxError<TTextBox>(
        this TTextBox textBox,
        Window window,
        CreateUserView createUserView,
        string text,
        ITestOutputHelper output
    ) where TTextBox : TextBox
    {
        return textBox.Case(() => window.SetKeyTextInput(text, output))
            .MustHasError()
            .Case(
                () => createUserView.FindControl<GlassCard>(ElementNames.CreateUserCard)
                    .ThrowIfNull()
                    .MustWidth(400)
            )
            .ClearText(window, output);
    }

    public static TTextBox ValidateCreateUserViewTextBox<TTextBox>(
        this TTextBox textBox,
        Window window,
        CreateUserView createUserView,
        string text,
        ITestOutputHelper output
    ) where TTextBox : TextBox
    {
        return textBox.Case(() => window.SetKeyTextInput(text, output))
            .MustNotHasError()
            .Case(
                () => createUserView.FindControl<GlassCard>(ElementNames.CreateUserCard)
                    .ThrowIfNull()
                    .MustWidth(400)
            )
            .ClearText(window, output);
    }
}