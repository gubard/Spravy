using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class MultiToDoItemsView : ReactiveUserControl<MultiToDoItemsViewModel>
{
    public const string ContentContentControlName = "content-content-control";

    public MultiToDoItemsView()
    {
        InitializeComponent();
    }
}