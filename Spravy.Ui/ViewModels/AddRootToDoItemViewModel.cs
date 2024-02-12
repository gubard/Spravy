using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ProtoBuf;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AddRootToDoItemViewModel : NavigatableViewModelBase
{
    private string name = string.Empty;
    private ToDoItemType type;
    private string url = string.Empty;

    public AddRootToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; } = new(Enum.GetValues<ToDoItemType>());
    public ICommand InitializedCommand { get; }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public string Url
    {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

    [Inject]
    public required EditDescriptionContentViewModel DescriptionContent { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    public override string ViewId => TypeCache<AddRootToDoItemViewModel>.Type.Name;

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var setting = await ObjectStorage.GetObjectOrDefaultAsync<AddRootToDoItemViewModelSetting>(ViewId)
            .ConfigureAwait(false);

        await SetStateAsync(setting).ConfigureAwait(false);
    }

    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new AddRootToDoItemViewModelSetting(this));
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<AddRootToDoItemViewModelSetting>();

        await this.InvokeUIAsync(
            () =>
            {
                Name = s.Name;
                Type = s.Type;
                Url = s.Url;
                DescriptionContent.Description = s.Description;
                DescriptionContent.Type = s.DescriptionType;
            }
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
            Name = viewModel.Name;
            Type = viewModel.Type;
            Url = viewModel.Url;
            Description = viewModel.DescriptionContent.Description;
            DescriptionType = viewModel.DescriptionContent.Type;
        }

        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(2)]
        public ToDoItemType Type { get; set; }

        [ProtoMember(3)]
        public string Url { get; set; } = string.Empty;

        [ProtoMember(4)]
        public string Description { get; set; } = string.Empty;

        [ProtoMember(5)]
        public DescriptionType DescriptionType { get; set; }
    }
}