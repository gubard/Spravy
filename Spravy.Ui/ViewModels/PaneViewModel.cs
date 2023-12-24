using Ninject;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase
{
    [Inject]
    public required AccountNotify Account { get; init; }
}