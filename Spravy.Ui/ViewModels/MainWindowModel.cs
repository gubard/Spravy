using Ninject;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainWindowModel : ViewModelBase, IScreen
{
    [Inject]
    public required RoutingState Router { get; init; }
}