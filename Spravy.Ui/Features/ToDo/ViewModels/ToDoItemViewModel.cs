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
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private readonly TaskWork refreshToDoItemWork;
    private readonly TaskWork refreshWork;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    private bool isBusy;

    public ToDoItemViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        refreshToDoItemWork = TaskWork.Create(RefreshToDoItemCore);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel?.Dispose();
            toDoSubItemsViewModel = value;

            Disposables.Add(toDoSubItemsViewModel.List
               .WhenAnyValue(x => x.IsMulti)
               .Skip(1)
               .Subscribe(_ => UpdateCommandsAsync(CancellationToken.None)));

            Disposables.Add(toDoSubItemsViewModel);
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
            Disposables.Add(pageHeaderViewModel);
        }
    }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Reactive]
    public object[] Path { get; set; } = Array.Empty<object>();

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Id}";
    }

    public bool IsDescriptionPlainText
    {
        get => DescriptionType == DescriptionType.PlainText;
    }

    public bool IsDescriptionMarkdownText
    {
        get => DescriptionType == DescriptionType.Markdown;
    }

    [Reactive]
    public bool IsFavorite { get; set; }

    [Reactive]
    public ToDoItemStatus Status { get; set; }

    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public Guid? ReferenceId { get; set; }

    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public Guid? ParentId { get; set; }
    
    public Guid CurrentId
    {
        get => ReferenceId ?? Id;
    }
    
    public bool IsNavigateToParent
    {
        get => true;
    }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public string Description { get; set; } = string.Empty;

    [Reactive]
    public DescriptionType DescriptionType { get; set; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore(cancellationToken).ConfigureAwait(false);
    }

    [Reactive]
    public string Link { get; set; } = string.Empty;

    [Reactive]
    public ToDoItemType Type { get; set; }

    private async ValueTask<Result> RefreshCore(CancellationToken cancellationToken)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return RefreshPathAsync(cancellationToken)
           .IfSuccessAllAsync(cancellationToken, () => RefreshToDoItemAsync().ConfigureAwait(false),
                () => RefreshToDoItemChildrenAsync(cancellationToken), () => UpdateCommandsAsync(cancellationToken));
    }

    private async ValueTask<Result> RefreshToDoItemAsync()
    {
        await refreshToDoItemWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        return ToDoService.GetToDoItemAsync(Id, cancellationToken)
           .IfSuccessAsync(item => this.InvokeUIBackgroundAsync(() =>
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
            }), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshPathAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetParentsAsync(Id, cancellationToken)
           .IfSuccessAsync(
                parents => this.InvokeUIBackgroundAsync(() =>
                    Path = new RootItem().To<object>()
                       .ToEnumerable()
                       .Concat(parents.ToArray().Select(x => Mapper.Map<ToDoItemParentNotify>(x)))
                       .ToArray()), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        Disposables.Add(this.WhenAnyValue(x => x.Name)
           .Subscribe(x =>
            {
                if (PageHeaderViewModel is null)
                {
                    return;
                }

                PageHeaderViewModel.Header = x;
            }));

        Disposables.Add(this.WhenAnyValue(x => x.DescriptionType)
           .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDescriptionPlainText));
                this.RaisePropertyChanged(nameof(IsDescriptionMarkdownText));
            }));

        return ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken),
                cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        pageHeaderViewModel.RightCommand = CommandStorage.ShowToDoSettingItem.WithParam(this);

        return ObjectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken)
           .IfSuccessAllAsync(cancellationToken, () => RefreshAsync(cancellationToken));
    }

    private ConfiguredValueTaskAwaitable<Result> UpdateCommandsAsync(CancellationToken cancellationToken)
    {
        return refreshToDoItemWork.Current.IfSuccessAsync(() => this.InvokeUIBackgroundAsync(() =>
        {
            if (ToDoSubItemsViewModel.List.IsMulti)
            {
                PageHeaderViewModel.SetMultiCommands(ToDoSubItemsViewModel);
            }
            else
            {
                PageHeaderViewModel.Commands.Clear();
                var toFavoriteCommand = CommandStorage.AddToDoItemToFavoriteItem.WithParam(CurrentId);
                PageHeaderViewModel.Commands.Add(CommandStorage.AddToDoItemChildItem.WithParam(this));
                PageHeaderViewModel.Commands.Add(CommandStorage.DeleteToDoItemItem.WithParam(this));

                if (!Link.IsNullOrWhiteSpace())
                {
                    PageHeaderViewModel.Commands.Add(CommandStorage.OpenLinkItem.WithParam(this));
                }

                if (IsCan != ToDoItemIsCan.None)
                {
                    PageHeaderViewModel.Commands.Add(CommandStorage.SwitchCompleteToDoItemItem.WithParam(this));
                }

                PageHeaderViewModel.Commands.Add(CommandStorage.CloneToDoItemItem.WithParam(this));

                if (IsFavorite)
                {
                    toFavoriteCommand = CommandStorage.RemoveToDoItemFromFavoriteItem.WithParam(CurrentId);
                }

                PageHeaderViewModel.Commands.Add(toFavoriteCommand);
                PageHeaderViewModel.Commands.Add(CommandStorage.NavigateToLeafItem.WithParam(CurrentId));
                PageHeaderViewModel.Commands.Add(CommandStorage.SetToDoParentItemItem.WithParam(this));
                PageHeaderViewModel.Commands.Add(CommandStorage.MoveToDoItemToRootItem.WithParam(this));
                PageHeaderViewModel.Commands.Add(CommandStorage.ToDoItemToStringItem.WithParam(this));
                PageHeaderViewModel.Commands.Add(CommandStorage.ResetToDoItemItem.WithParam(this));

                PageHeaderViewModel.Commands.Add(CommandStorage.ToDoItemRandomizeChildrenOrderIndexItem
                   .WithParam(this));
            }
        }), cancellationToken);
    }

    public override Result Stop()
    {
        refreshToDoItemWork.Cancel();
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<ToDoItemViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUIBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }), cancellationToken);
    }

    [ProtoContract]
    private class ToDoItemViewModelSetting : IViewModelSetting<ToDoItemViewModelSetting>
    {
        public ToDoItemViewModelSetting(ToDoItemViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public ToDoItemViewModelSetting()
        {
        }
        
        static ToDoItemViewModelSetting()
        {
            Default = new ()
            {
                GroupBy = GroupBy.ByStatus,
            };
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; }

        [ProtoMember(2)]
        public bool IsMulti { get; set; }

        public static ToDoItemViewModelSetting Default { get; }
    }
}