using Avalonia.Controls;
using Material.Styles.Controls;
using Spravy.Domain.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.Views;

namespace Spravy.Tests.Extensions;

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
            .Case(
                () => createUserView.FindControl<Card>(ElementNames.CreateUserCard)
                    .ThrowIfNull()
                    .MustWidth(300)
            )
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
            .Case(
                () => createUserView.FindControl<Card>(ElementNames.CreateUserCard)
                    .ThrowIfNull()
                    .MustWidth(300)
            )
            .ClearText(window);
    }
}