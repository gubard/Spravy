using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddRootToDoItemViewModel : NavigatableViewModelBase
{
    public AddRootToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required ToDoItemContentViewModel ToDoItemContent { get; init; }

    [Inject]
    public required EditDescriptionContentViewModel DescriptionContent { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    public override string ViewId => TypeCache<AddRootToDoItemViewModel>.Type.Name;

    private ValueTask<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<AddRootToDoItemViewModelSetting>(ViewId)
            .ConfigureAwait(false)
            .IfSuccessAsync(s => SetStateAsync(s).ConfigureAwait(false));
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new AddRootToDoItemViewModelSetting(this));
    }

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        return setting.CastObject<AddRootToDoItemViewModelSetting>()
            .IfSuccessAsync(
                s => this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            ToDoItemContent.Name = s.Name;
                            ToDoItemContent.Type = s.Type;
                            ToDoItemContent.Link = s.Link;
                            DescriptionContent.Description = s.Description;
                            DescriptionContent.Type = s.DescriptionType;
                        }
                    )
                    .ConfigureAwait(false)
            );
    }

    [ProtoContract]
    class AddRootToDoItemViewModelSetting
    {
        public AddRootToDoItemViewModelSetting()
        {
        }

        public AddRootToDoItemViewModelSetting(AddRootToDoItemViewModel viewModel)
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