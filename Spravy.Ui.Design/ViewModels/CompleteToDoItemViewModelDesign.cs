using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class CompleteToDoItemViewModelDesign : CompleteToDoItemViewModel
{
    public CompleteToDoItemViewModelDesign()
    {
        CompleteStatuses.AddRange(Enum.GetValues<CompleteStatus>().Select(x => new Ref<CompleteStatus>(x)));
    }
}