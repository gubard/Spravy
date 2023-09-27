using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemToStringSettingsView : ReactiveUserControl<ToDoItemToStringSettingsViewModel>
{
    public ToDoItemToStringSettingsView()
    {
        InitializeComponent();
    }
}