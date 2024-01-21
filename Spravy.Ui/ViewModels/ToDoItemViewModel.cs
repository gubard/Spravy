using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ProtoBuf;
using ReactiveUI;
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

namespace Spravy.Ui.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase,
    IToDoItemOrderChanger,
    ICanComplete,
    IToDoLinkProperty,
    IToDoDescriptionProperty,
    IToDoSettingsProperty,
    IToDoNameProperty,
    IDeletable,
    ISetToDoParentItemParams
{
    private string name = string.Empty;
    private Guid id;
    private string description = string.Empty;
    private ToDoItemType type;
    private bool isFavorite;
    private string link = string.Empty;
    private ToDoItemIsCan isCan;
    private readonly TaskWork refreshToDoItemWork;
    private readonly TaskWork refreshWork;
    private ToDoItemStatus status;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private Guid? parentId;

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
            pageHeaderViewModel.RightCommand = CommandStorage.SetToDoItemNameItem.WithParam(this);
        }
    }

    public object Header => Name;

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathViewModel PathViewModel { get; set; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    public Guid? ParentId
    {
        get => parentId;
        set => this.RaiseAndSetIfChanged(ref parentId, value);
    }

    public bool IsNavigateToParent => true;
    public override string ViewId => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Id}";

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public string Link
    {
        get => link;
        set => this.RaiseAndSetIfChanged(ref link, value);
    }

    public ToDoItemIsCan IsCan
    {
        get => isCan;
        set => this.RaiseAndSetIfChanged(ref isCan, value);
    }

    public ToDoItemStatus Status
    {
        get => status;
        set => this.RaiseAndSetIfChanged(ref status, value);
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync();
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        await RefreshPathAsync(cancellationToken);

        await Task.WhenAll(
                RefreshToDoItemAsync(),
                RefreshToDoItemChildrenAsync(cancellationToken),
                UpdateCommandsAsync()
            )
            .WaitAsync(cancellationToken);
    }

    private Task RefreshToDoItemAsync()
    {
        return refreshToDoItemWork.RunAsync();
    }

    private async Task RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        var item = await ToDoService.GetToDoItemAsync(Id, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
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
            }
        );
    }


    private async Task RefreshPathAsync(CancellationToken cancellationToken)
    {
        var parents = await ToDoService.GetParentsAsync(Id, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                PathViewModel.Items.Clear();
                PathViewModel.Items.Add(new RootItem());
                PathViewModel.Items.AddRange(parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
            }
        );
    }

    private async Task RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken).ConfigureAwait(false);
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var setting = await ObjectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId)
            .ConfigureAwait(false);

        await SetStateAsync(setting).ConfigureAwait(false);
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task UpdateCommandsAsync()
    {
        await refreshToDoItemWork.Current;

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                PageHeaderViewModel.Commands.Clear();

                if (ToDoSubItemsViewModel.List.IsMulti)
                {
                    PageHeaderViewModel.Commands.Add(
                        CommandStorage.MultiCompleteToDoItemsItem.WithParam(
                            ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                        )
                    );
                    PageHeaderViewModel.Commands.Add(
                        CommandStorage.MultiSetTypeToDoItemsItem.WithParam(
                            ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                        )
                    );
                    PageHeaderViewModel.Commands.Add(
                        CommandStorage.MultiSetParentToDoItemsItem.WithParam(
                            ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                        )
                    );
                    PageHeaderViewModel.Commands.Add(
                        CommandStorage.MultiMoveToDoItemsToRootItem.WithParam(
                            ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                        )
                    );
                }
                else
                {
                    var toFavoriteCommand = CommandStorage.AddToDoItemToFavoriteItem.WithParam(Id);

                    PageHeaderViewModel.Commands.Add(
                        CommandStorage.AddToDoItemChildItem.WithParam(
                            PathViewModel.Items[^1].As<ToDoItemParentNotify>().ThrowIfNull().Id
                        )
                    );

                    PageHeaderViewModel.Commands.Add(CommandStorage.DeleteToDoItemItem.WithParam(this));

                    if (IsCan != ToDoItemIsCan.None)
                    {
                        PageHeaderViewModel.Commands.Add(CommandStorage.CompleteToDoItemItem.WithParam(this));
                    }

                    if (Type != ToDoItemType.Group)
                    {
                        PageHeaderViewModel.Commands.Add(CommandStorage.ShowToDoSettingItem.WithParam(this));
                    }

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
        );
    }

    public override void Stop()
    {
        refreshToDoItemWork.Cancel();
        refreshWork.Cancel();
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this));
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<ToDoItemViewModelSetting>();

        await this.InvokeUIAsync(
            () =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }
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