using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : ViewModelBase
{
    public PasswordItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required IPasswordService PasswordService { get; init; }

    [Reactive]
    public Guid Id { get; set; }

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

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var item = await PasswordService.GetPasswordItemAsync(Id, cancellationToken);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                Name = item.Name;
                Regex = item.Regex;
                Key = item.Key;
                Length = item.Length;
                IsAvailableUpperLatin = item.IsAvailableUpperLatin;
                IsAvailableLowerLatin = item.IsAvailableLowerLatin;
                IsAvailableNumber = item.IsAvailableNumber;
                IsAvailableSpecialSymbols = item.IsAvailableSpecialSymbols;
                CustomAvailableCharacters = item.CustomAvailableCharacters;
            }
        );
    }
}