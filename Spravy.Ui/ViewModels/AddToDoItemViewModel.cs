using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Material.Icons;
using Ninject;
using ProtoBuf;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddToDoItemViewModel : NavigatableViewModelBase
{
    private Guid parentId;
    private object[] path = Array.Empty<object>();

    public AddToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    public object[] Path
    {
        get => path;
        set => this.RaiseAndSetIfChanged(ref path, value);
    }

    public Guid ParentId
    {
        get => parentId;
        set => this.RaiseAndSetIfChanged(ref parentId, value);
    }

    [Inject]
    public required ToDoItemContentViewModel ToDoItemContent { get; init; }

    [Inject]
    public required EditDescriptionContentViewModel DescriptionContent { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }


    public override string ViewId => $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{ParentId}";

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var setting = await ObjectStorage.GetObjectOrDefaultAsync<AddToDoItemViewModelSetting>(ViewId)
            .ConfigureAwait(false);

        await SetStateAsync(setting).ConfigureAwait(false);
        var parents = await ToDoService.GetParentsAsync(ParentId, cancellationToken).ConfigureAwait(false);

        var path = MaterialIconKind.Home.As<object>()
            .ToEnumerable()
            .Concat(parents.Select(x => x.Name))
            .Select(x => x.ThrowIfNull())
            .ToArray();

        await this.InvokeUIBackgroundAsync(() => Path = path);
    }


    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new AddToDoItemViewModelSetting(this));
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<AddToDoItemViewModelSetting>();

        await this.InvokeUIAsync(
            () =>
            {
                ToDoItemContent.Name = s.Name;
                ToDoItemContent.Type = s.Type;
                ToDoItemContent.Link = s.Link;
                DescriptionContent.Description = s.Description;
                DescriptionContent.Type = s.DescriptionType;
            }
        );
    }

    [ProtoContract]
    class AddToDoItemViewModelSetting
    {
        public AddToDoItemViewModelSetting()
        {
        }

        public AddToDoItemViewModelSetting(AddToDoItemViewModel viewModel)
        {
            Name = viewModel.ToDoItemContent.Name;
            Type = viewModel.ToDoItemContent.Type;
            Link = viewModel.ToDoItemContent.Link;
            Description = viewModel.DescriptionContent.Description;
            DescriptionType = viewModel.DescriptionContent.Type;
        }

        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(2)]
        public ToDoItemType Type { get; set; }

        [ProtoMember(3)]
        public string Link { get; set; } = string.Empty;

        [ProtoMember(4)]
        public string Description { get; set; } = string.Empty;

        [ProtoMember(5)]
        public DescriptionType DescriptionType { get; set; }
    }
}