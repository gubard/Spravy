using System;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    ValueTask<Result<INavigatable?>> NavigateBackAsync(CancellationToken cancellationToken);

    ValueTask<Result> NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken)
        where TViewModel : INavigatable;

    ValueTask<Result> NavigateToAsync(Type type, CancellationToken cancellationToken);

    ValueTask<Result> NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : INavigatable;

    ValueTask<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken)
        where TViewModel : INavigatable;
}