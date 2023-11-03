using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    private Guid id;
    private bool isAfter = true;
    private ToDoItemNotify? selectedItem;

    public ChangeToDoItemOrderIndexViewModel()
    {
        InitializedCommand = CreateInitializedCommand(InitializedAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public AvaloniaList<ToDoItemNotify> Items { get; } = new();

    public ToDoItemNotify? SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public bool IsAfter
    {
        get => isAfter;
        set => this.RaiseAndSetIfChanged(ref isAfter, value);
    }

    private async Task InitializedAsync()
    {
        var items = await ToDoService.GetSiblingsAsync(Id);
        Items.Clear();
        Items.AddRange(Mapper.Map<IEnumerable<ToDoItemNotify>>(items));
    }
}