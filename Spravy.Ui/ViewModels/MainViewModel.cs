using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        BackCommand = CreateInitializedCommand(TaskWork.Create(BackAsync).RunAsync);
    }

    public ICommand BackCommand { get; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    private async Task BackAsync(CancellationToken cancellationToken)
    {
        await Navigator.NavigateBackAsync(cancellationToken).ConfigureAwait(false);
    }
}