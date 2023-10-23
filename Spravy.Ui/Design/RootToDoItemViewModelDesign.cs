using System;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Ui.Design;

public class RootToDoItemViewModelDesign : RootToDoItemViewModel
{
    public RootToDoItemViewModelDesign()
    {
        ToDoSubItemsView = new ToDoSubItemsView()
        {
            ViewModel = new ToDoSubItemsViewModelDesign
            {
                Mapper = null,
                Navigator = null,
                DialogViewer = null,
                ToDoService = null,
                Completed =
                {
                    new ToDoSubItemGroupNotify
                    {
                        Name = "ToDoSubItemGroupNotify",
                    },
                    new ToDoSubItemPeriodicityNotify
                    {
                        Name = "ToDoSubItemPeriodicityNotify",
                    },
                    new ToDoSubItemPlannedNotify
                    {
                        Name = "ToDoSubItemPlannedNotify",
                    },
                    new ToDoSubItemValueNotify
                    {
                        Name = "ToDoSubItemValueNotify",
                    },
                    new ToDoSubItemPeriodicityOffsetNotify
                    {
                        Name = "ToDoSubItemPeriodicityOffsetNotify",
                        /*Active = new ActiveToDoItemNotify
                        {
                            Name = "Active"
                        },*/
                        CompletedCount = 2,
                        FailedCount = 3,
                        SkippedCount = 6,
                        LastCompleted = DateTimeOffset.Now
                    }
                }
            },
        };
    }
}