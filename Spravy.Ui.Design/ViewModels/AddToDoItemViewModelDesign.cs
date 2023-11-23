﻿using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class AddToDoItemViewModelDesign : AddToDoItemViewModel
{
    public AddToDoItemViewModelDesign()
    {
        PathViewModel = new PathViewModel
        {
            Navigator = null,
            DialogViewer = null
        };

        ToDoService = new ToDoServiceDesign(
            Enumerable.Empty<IToDoSubItem>(),
            Enumerable.Empty<IToDoSubItem>(),
           null,
            Enumerable.Empty<ToDoShortItem>(),
            Enumerable.Empty<IToDoSubItem>()
        );
    }
}