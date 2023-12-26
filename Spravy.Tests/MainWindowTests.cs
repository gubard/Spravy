using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using FluentAssertions;
using Spravy.Domain.Extensions;
using Spravy.Tests.Extensions;
using Spravy.Tests.Helpers;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Tests;

public class MainWindowTests
{
    [AvaloniaFact]
    public void Should_Type_Text_Into_TextBox()
    {
        WindowHelper.CreateWindow()
            .ShowWindow()
            .Case(w => w.Height = 1000)
            .Case(w => w.Width = 1000)
            .Case(
                w => w.GetCurrentView<LoginView, LoginViewModel>()
                    .FindControl<Button>("CreateUserButton")
                    .ThrowIfNull()
                    .Case(b => w.MouseDown(b.Bounds.Position, MouseButton.Left))
                    .Case(b => w.MouseUp(b.Bounds.Position, MouseButton.Left))
            )
            .GetCurrentView<CreateUserView, CreateUserViewModel>();
    }
}