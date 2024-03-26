using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class AddPasswordItemViewModel : NavigatableViewModelBase
{
    public AddPasswordItemViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<AddPasswordItemViewModel>.Type.Name;

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public string Regex { get; set; } = string.Empty;

    [Reactive]
    public string Key { get; set; } = string.Empty;

    [Reactive]
    public ushort Length { get; set; } = 512;

    [Reactive]
    public bool IsAvailableUpperLatin { get; set; } = true;
    
    [Reactive]
    public bool IsAvailableLowerLatin { get; set; } = true;

    [Reactive]
    public bool IsAvailableNumber { get; set; } = true;

    [Reactive]
    public bool IsAvailableSpecialSymbols { get; set; } = true;

    [Reactive]
    public string CustomAvailableCharacters { get; set; } = string.Empty;

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