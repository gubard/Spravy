using System;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface IOpenerLink
{
    Task OpenLinkAsync(Uri link);
}