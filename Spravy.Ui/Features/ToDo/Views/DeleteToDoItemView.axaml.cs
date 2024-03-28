using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class DeleteToDoItemView : ReactiveUserControl<DeleteToDoItemViewModel>
{
    public DeleteToDoItemView()
    {
        InitializeComponent();
    }
}