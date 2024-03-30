using System.Threading.Tasks;
using Avalonia.Collections;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Domain.ValidationResults;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ErrorHandling.ViewModels;

public class ErrorViewModel : NavigatableViewModelBase
{
    public ErrorViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<ErrorViewModel>.Type.Name;

    public AvaloniaList<ValidationResult> ValidationResults { get; } = new();

    public override void Stop()
    {
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }
}