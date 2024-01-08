using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Material.Styles.Controls;
using Spravy.Domain.Extensions;
using Spravy.Tests.Extensions;
using Spravy.Tests.Helpers;

namespace Spravy.Tests;

public class MainWindowTests
{
    [AvaloniaFact]
    public void Should_Type_Text_Into_TextBox()
    {
        WindowHelper.CreateWindow()
            .TryCatch(
                w => w.SetSize(1000, 1000)
                    .ShowWindow()
                    .Case(
                        () => w.NavigateToCreateUserView()
                            .Case(
                                c => c.FindControl<TextBox>(ElementNames.EmailTextBox)
                                    .ThrowIfNull()
                                    .MustFocused()
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength4))
                                    .MustHasError()
                                    .Case(
                                        () => c.FindControl<Card>(ElementNames.CreateUserCard)
                                            .ThrowIfNull()
                                            .MustWidth(300)
                                    )
                                    .ClearText(w)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength3))
                                    .MustHasError()
                                    .Case(
                                        () => c.FindControl<Card>(ElementNames.CreateUserCard)
                                            .ThrowIfNull()
                                            .MustWidth(300)
                                    )
                                    .ClearText(w)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength51))
                                    .MustHasError()
                                    .Case(
                                        () => c.FindControl<Card>(ElementNames.CreateUserCard)
                                            .ThrowIfNull()
                                            .MustWidth(300)
                                    )
                                    .ClearText(w)
                                    .Case(() => w.SetKeyTextInput(TextHelper.TextLength50))
                                    .MustHasError()
                                    .Case(
                                        () => c.FindControl<Card>(ElementNames.CreateUserCard)
                                            .ThrowIfNull()
                                            .MustWidth(300)
                                    )
                                    .ClearText(w)
                                    .Case(() => w.SetKeyTextInput(TextHelper.EmailLength51))
                                    .MustHasError()
                                    .Case(
                                        () => c.FindControl<Card>(ElementNames.CreateUserCard)
                                            .ThrowIfNull()
                                            .MustWidth(300)
                                    )
                                    .ClearText(w)
                                    .Case(() => w.SetKeyTextInput(TextHelper.EmailLength50))
                                    .MustNotHasError()
                                    .Case(
                                        () => c.FindControl<Card>(ElementNames.CreateUserCard)
                                            .ThrowIfNull()
                                            .MustWidth(300)
                                    )
                            )
                    )
                    .RunJobsAll()
                    .SaveFrame(),
                (w, _) => w.SaveFrame()
            );
    }
}