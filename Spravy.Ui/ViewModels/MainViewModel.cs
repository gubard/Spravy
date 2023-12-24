using Ninject;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }
}