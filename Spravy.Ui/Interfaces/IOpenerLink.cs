using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface IOpenerLink
{
    ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken);
}