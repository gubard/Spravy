using Spravy.Ui.Design.Helpers;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class ChangeToDoItemOrderIndexViewModelDesign : ChangeToDoItemOrderIndexViewModel
{
    public ChangeToDoItemOrderIndexViewModelDesign()
    {
        Mapper = ConstDesign.Mapper;

        ToDoService = null;
    }
}