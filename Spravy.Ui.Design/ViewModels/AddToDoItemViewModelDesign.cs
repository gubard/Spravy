using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class AddToDoItemViewModelDesign : AddToDoItemViewModel
{
    public AddToDoItemViewModelDesign()
    {
        PathViewModel = new PathViewModel
        {
            Navigator = null,
            DialogViewer = null
        };
    }
}