using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemSettingsView : ReactiveUserControl<ToDoItemSettingsViewModel>
{
    public ToDoItemSettingsView()
    {
        InitializeComponent();
    }
}