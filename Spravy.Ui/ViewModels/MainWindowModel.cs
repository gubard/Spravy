using ReactiveUI;
using Spravy.Domain.Attributes;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainWindowModel : ViewModelBase, IScreen
{
    [Inject]
    public RoutingState Router { get; init; }
}