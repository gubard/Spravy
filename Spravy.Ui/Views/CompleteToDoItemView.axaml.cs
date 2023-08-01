using Avalonia.Markup.Xaml;
using ExtensionFramework.AvaloniaUi.ReactiveUI.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class CompleteToDoItemView : ReactiveUserControl<CompleteToDoItemViewModel>
{
    public CompleteToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}