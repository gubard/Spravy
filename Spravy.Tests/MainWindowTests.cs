using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FluentAssertions;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Views;

namespace Spravy.Tests;

public class MainWindowTests
{
    [AvaloniaFact]
    public void Should_Type_Text_Into_TextBox()
    {
        var window = DiHelper.Kernel.ThrowIfNull().Get<Window>();
        window.Should().BeAssignableTo<MainWindow>();
        window.Content.Should().BeAssignableTo<MainView>();
    }
}