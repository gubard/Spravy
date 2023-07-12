using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Spravy.Ui.Views;

public partial class DayOfWeekSelectorView : UserControl
{
    public DayOfWeekSelectorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}