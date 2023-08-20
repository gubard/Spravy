using System;
using ReactiveUI;

namespace Spravy.Ui.Interfaces;

public interface IExceptionViewModel : IRoutableViewModel
{
    Exception? Exception { get; set; }
}