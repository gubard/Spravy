using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ProtoBuf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase,
    IToDoItemOrderChanger,
    ICanCompleteProperty,
    IToDoLinkProperty,
    IToDoDescriptionProperty,
    IToDoSettingsProperty,
    IToDoNameProperty,
    IDeletable,
    ISetToDoParentItemParams,
    ILink
{
    private readonly TaskWork refreshToDoItemWork;
    private readonly TaskWork refreshWork;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private bool isBusy;

    public ToDoItemViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        refreshToDoItemWork = TaskWork.Create(RefreshToDoItemCore);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);

        this.WhenAnyValue(x => x.Name)
            .Subscribe(
                x =>
                {
                    if (PageHeaderViewModel is null)
                    {
                        return;
                    }

                    PageHeaderViewModel.Header = x;
                }
            );

        this.WhenAnyValue(x => x.DescriptionType)
            .Subscribe(
                _ =>
                {
                    this.RaisePropertyChanged(nameof(IsDescriptionPlainText));
                    this.RaisePropertyChanged(nameof(IsDescriptionMarkdownText));
                }
            );
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;
            toDoSubItemsViewModel.List.WhenAnyValue(x => x.IsMulti)
                .Skip(1)
                .Subscribe(_ => UpdateCommandsAsync());
        }
    }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = Name;
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
            pageHeaderViewModel.RightCommand = CommandStorage.ShowToDoSettingItem.WithParam(this);
        }
    }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Reactive]
    public Guid? ParentId { get; set; }

    [Reactive]
    public object[] Path { get; set; } = Array.Empty<object>();

    public bool IsNavigateToParent => true;
    public override string ViewId => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Id}";
    public bool IsDescriptionPlainText => DescriptionType == DescriptionType.PlainText;
    public bool IsDescriptionMarkdownText => DescriptionType == DescriptionType.Markdown;

    [Reactive]
    public bool IsFavorite { get; set; }

    [Reactive]
    public ToDoItemType Type { get; set; }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public string Description { get; set; } = string.Empty;

    [Reactive]
    public DescriptionType DescriptionType { get; set; }

    [Reactive]
    public string Link { get; set; } = string.Empty;

    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }

    public bool IsBusy
    {
        get => isBusy;
        set => this.RaiseAndSetIfChanged(ref isBusy, value);
    }

    [Reactive]
    public ToDoItemStatus Status { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore(CancellationToken cancellationToken)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return RefreshPathAsync(cancellationToken)
            .IfSuccessAllAsync(
                () => RefreshToDoItemAsync().ConfigureAwait(false),
                () => RefreshToDoItemChildrenAsync(cancellationToken),
                UpdateCommandsAsync
            );
    }

    private async ValueTask<Result> RefreshToDoItemAsync()
    {
        await refreshToDoItemWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        return ToDoService.GetToDoItemAsync(Id, cancellationToken)
            .IfSuccessAsync(
                item =>
                    this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            Link = item.Link?.AbsoluteUri ?? string.Empty;
                            Description = item.Description;
                            Name = item.Name;
                            Type = item.Type;
                            IsCan = item.IsCan;
                            IsFavorite = item.IsFavorite;
                            Status = item.Status;
                            ParentId = item.ParentId;
                            DescriptionType = item.DescriptionType;
                        }
                    )
            );
    }


    private ConfiguredValueTaskAwaitable<Result> RefreshPathAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetParentsAsync(Id, cancellationToken)
            .IfSuccessAsync(
                parents =>
                    this.InvokeUIBackgroundAsync(
                        () => Path = new RootItem().To<object>()
                            .ToEnumerable()
                            .Concat(parents.ToArray().Select(x => Mapper.Map<ToDoItemParentNotify>(x)))
                            .ToArray()
                    )
            );
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken)
            .IfSuccessAsync(
                ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken)
            );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId)
            .IfSuccessAsync(SetStateAsync)
            .IfSuccessAllAsync(() => RefreshAsync(cancellationToken));
    }

    private ConfiguredValueTaskAwaitable<Result> UpdateCommandsAsync()
    {
        return refreshToDoItemWork.Current
            .IfSuccessAsync(
                () => this.InvokeUIBackgroundAsync(
                    () =>
                    {
                        if (ToDoSubItemsViewModel.List.IsMulti)
                        {
                            PageHeaderViewModel.SetMultiCommands(ToDoSubItemsViewModel);
                        }
                        else
                        {
                            PageHeaderViewModel.Commands.Clear();
                            var toFavoriteCommand = CommandStorage.AddToDoItemToFavoriteItem.WithParam(Id);
                            PageHeaderViewModel.Commands.Add(CommandStorage.AddToDoItemChildItem.WithParam(this));
                            PageHeaderViewModel.Commands.Add(CommandStorage.DeleteToDoItemItem.WithParam(this));

                            if (!Link.IsNullOrWhiteSpace())
                            {
                                PageHeaderViewModel.Commands.Add(CommandStorage.OpenLinkItem.WithParam(this));
                            }

                            if (IsCan != ToDoItemIsCan.None)
                            {
                                PageHeaderViewModel.Commands.Add(
                                    CommandStorage.SwitchCompleteToDoItemItem.WithParam(this)
                                );
                            }

                            PageHeaderViewModel.Commands.Add(CommandStorage.CloneToDoItemItem.WithParam(this));

                            if (IsFavorite)
                            {
                                toFavoriteCommand = CommandStorage.RemoveToDoItemFromFavoriteItem.WithParam(Id);
                            }

                            PageHeaderViewModel.Commands.Add(toFavoriteCommand);
                            PageHeaderViewModel.Commands.Add(CommandStorage.NavigateToLeafItem.WithParam(Id));
                            PageHeaderViewModel.Commands.Add(CommandStorage.SetToDoParentItemItem.WithParam(this));
                            PageHeaderViewModel.Commands.Add(CommandStorage.MoveToDoItemToRootItem.WithParam(this));
                            PageHeaderViewModel.Commands.Add(CommandStorage.ToDoItemToStringItem.WithParam(this));
                            PageHeaderViewModel.Commands.Add(CommandStorage.ResetToDoItemItem.WithParam(this));
                            PageHeaderViewModel.Commands.Add(
                                CommandStorage.ToDoItemRandomizeChildrenOrderIndexItem.WithParam(this)
                            );
                        }
                    }
                )
            );
    }

    public override Result Stop()
    {
        refreshToDoItemWork.Cancel();
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting)
    {
        return setting.CastObject<ToDoItemViewModelSetting>()
            .IfSuccessAsync(
                s => this.InvokeUIBackgroundAsync(
                    () =>
                    {
                        ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                        ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
                    }
                )
            );
    }

    [ProtoContract]
    class ToDoItemViewModelSetting
    {
        public ToDoItemViewModelSetting(ToDoItemViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public ToDoItemViewModelSetting()
        {
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

        [ProtoMember(2)]
        public bool IsMulti { get; set; }
    }
}