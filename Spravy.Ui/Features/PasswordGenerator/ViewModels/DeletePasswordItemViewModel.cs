using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Features.Localizations.Models;
using Spravy.Ui.Models;
using System;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Ninject;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.Ui.Extensions;

namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : NavigatableViewModelBase
{
    public DeletePasswordItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        this.WhenAnyValue(x => x.PasswordItemName).Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)));
    }

    public override string ViewId => TypeCache<DeletePasswordItemViewModel>.Type.Name;

    public ICommand InitializedCommand { get; }

    [Reactive]
    public Guid PasswordItemId { get; set; }

    [Reactive]
    public string PasswordItemName { get; set; } = string.Empty;

    [Inject]
    public required IPasswordService PasswordService { get; init; }

    public HeaderView DeleteText =>
        new(
            "DeletePasswordItemView.Header",
            new
            {
                PasswordItemName
            }
        );

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

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return PasswordService.GetPasswordItemAsync(PasswordItemId, cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                DialogViewer,
                async value => { await this.InvokeUIBackgroundAsync(() => PasswordItemName = value.Name); }
            );
    }
}