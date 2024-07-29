using Spravy.Ui.Controls;

namespace Spravy.Integration.Tests.Extensions;

public static class CreateUserViewExtension
{
    public static TTextBox ValidateCreateUserViewTextBoxError<TTextBox>(
        this TTextBox textBox,
        Window window,
        CreateUserView createUserView,
        string text
    )
        where TTextBox : TextBox
    {
        return textBox
            .Case(() => window.SetKeyTextInput(text))
            .MustHasError()
            .Case(
                () =>
                    createUserView
                        .GetControl<BusyAreaControl>(ElementNames.CreateUserCard)
                        .MustWidth(400)
            )
            .ClearText(window);
    }

    public static TTextBox ValidateCreateUserViewTextBox<TTextBox>(
        this TTextBox textBox,
        Window window,
        CreateUserView createUserView,
        string text
    )
        where TTextBox : TextBox
    {
        return textBox
            .Case(() => window.SetKeyTextInput(text))
            .MustNotHasError()
            .Case(
                () =>
                    createUserView
                        .GetControl<BusyAreaControl>(ElementNames.CreateUserCard)
                        .MustWidth(400)
            )
            .ClearText(window);
    }
}
