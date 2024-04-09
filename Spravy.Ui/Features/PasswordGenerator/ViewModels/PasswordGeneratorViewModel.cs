using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using DynamicData;
using Ninject;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Domain.Extensions;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.Localizations.Models;
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
            pageHeaderViewModel.Header = new Header3View("PasswordGeneratorView.Header");
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
            Disposables.Add(pageHeaderViewModel);
        }
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting, CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync( CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return PasswordService.GetPasswordItemsAsync(cancellationToken)
            .IfSuccessAsync(
                items => this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            Items.Clear();
                            Items.AddRange(Mapper.Map<PasswordItemNotify[]>(items.ToArray()));
                        }
                    ),cancellationToken
            );
    }
}