using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;

namespace Spravy.Ui.ViewModels;

public class MainWindowModel : ViewModelBase, IScreen
{
    [Inject]
    public RoutingState Router { get; init; }
}