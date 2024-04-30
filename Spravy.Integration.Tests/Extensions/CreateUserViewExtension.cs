using Avalonia.Controls;
using Spravy.Domain.Extensions;
using Spravy.Integration.Tests.Helpers;
using Spravy.Ui.Views;
using SukiUI.Controls;

namespace Spravy.Integration.Tests.Extensions;

public static class CreateUserViewExtension
{
    public static TTextBox ValidateCreateUserViewTextBoxError<TTextBox>(
        this TTextBox textBox,
        Window window,
        CreateUserView createUserView,
        string text
    ) where TTextBox : TextBox
    {
        return textBox.Case(() => window.SetKeyTextInput(text))
           .MustHasError()
           .Case(() => createUserView.FindControl<GlassCard>(ElementNames.CreateUserCard).ThrowIfNull().MustWidth(400))
           .ClearText(window);
    }

    public static TTextBox ValidateCreateUserViewTextBox<TTextBox>(
        this TTextBox textBox,
        Window window,
        CreateUserView createUserView,
        string text
    ) where TTextBox : TextBox
    {
        return textBox.Case(() => window.SetKeyTextInput(text))
           .MustNotHasError()
           .Case(() => createUserView.FindControl<GlassCard>(ElementNames.CreateUserCard).ThrowIfNull().MustWidth(400))
           .ClearText(window);
    }
}