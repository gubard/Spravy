using System;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface IOpenerLink
{
    ValueTask<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken);
}