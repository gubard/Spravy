using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Features.Localizations.Models;
using Spravy.Ui.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Services;

namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase
{
    public DeletePasswordItemViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Reactive]
    public Guid PasswordItemId { get; set; }

    [Reactive]
    public string PasswordItemName { get; set; } = string.Empty;

    [Inject]
    public required IPasswordService PasswordService { get; init; }

    public Header4View DeleteText =>
        new(
            "DeletePasswordItemView.Header",
            new
            {
                PasswordItemName
            }
        );

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        Disposables.Add(
            this.WhenAnyValue(x => x.PasswordItemName).Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)))
        );

        return PasswordService.GetPasswordItemAsync(PasswordItemId, cancellationToken)
            .IfSuccessAsync(
                value => this.InvokeUIBackgroundAsync(() => PasswordItemName = value.Name)
            );
    }
}