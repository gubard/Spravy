using Avalonia.Controls;
using Avalonia.Input;
using Spravy.Domain.Extensions;

namespace Spravy.Tests.Extensions;

public static class TextBoxExtension
{
    public static TTextBox ClearText<TTextBox>(this TTextBox textBox, Window window)
        where TTextBox : TextBox
    {
        textBox.Text.ThrowIfNullOrEmpty();
        textBox.MustFocused();

        for (var i = 0; i < textBox.Text.ThrowIfNull().Length; i++)
        {
            window.SpravyKeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            window.RunJobsAll();
            window.SpravyKeyReleaseQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            window.RunJobsAll();
        }

        while (textBox.Text.ThrowIfNull().Length > 0)
        {
            window.SpravyKeyPressQwerty(PhysicalKey.Backspace, RawInputModifiers.None);
            window.RunJobsAll();
            window.SpravyKeyReleaseQwerty(PhysicalKey.Backspace, RawInputModifiers.None);
            window.RunJobsAll();
        }

        return textBox;
    }
}