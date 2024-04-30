using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : NavigatableViewModelBase, IToDoItemSearchProperties
{
    private readonly TaskWork refreshWork;

    public SearchViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<SearchViewModel>.Type.Name;
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Reactive]
    public string SearchText { get; set; } = string.Empty;

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return ToDoService.SearchToDoItemIdsAsync(SearchText, cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken),
                cancellationToken);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new SearchViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<SearchViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUIBackgroundAsync(() => SearchText = s.SearchText), cancellationToken);
    }

    [ProtoContract]
    private class SearchViewModelSetting : IViewModelSetting<SearchViewModelSetting>
    {
        public SearchViewModelSetting(SearchViewModel viewModel)
        {
            SearchText = viewModel.SearchText;
        }

        public SearchViewModelSetting()
        {
        }

        static SearchViewModelSetting()
        {
            Default = new();
        }

        [ProtoMember(1)]
        public string SearchText { get; set; } = string.Empty;

        public static SearchViewModelSetting Default { get; }
    }
}