using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ResetToDoItemView : ReactiveUserControl<ResetToDoItemViewModel>
{
    public ResetToDoItemView()
    {
        InitializeComponent();
    }
}