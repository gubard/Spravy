using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DeleteToDoItemView : ReactiveUserControl<DeleteToDoItemViewModel>
{
    public DeleteToDoItemView()
    {
        InitializeComponent();
    }
}