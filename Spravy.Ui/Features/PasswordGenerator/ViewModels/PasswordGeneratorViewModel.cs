using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.PasswordGenerator.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordGeneratorViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly PageHeaderViewModel pageHeaderViewModel;

    public PasswordGeneratorViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public override string ViewId => TypeCache<PasswordGeneratorViewModel>.Type.Name;

    public AvaloniaList<PasswordItemNotify> Items { get; } = new();
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IPasswordService PasswordService { get; init; }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = "Password generator";
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        }
    }

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

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

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return PasswordService.GetPasswordItemsAsync(cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                DialogViewer,
                async items =>
                {
                    await this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            Items.Clear();
                            Items.AddRange(Mapper.Map<PasswordItemNotify[]>(items.ToArray()));
                        }
                    );
                }
            );
    }
}