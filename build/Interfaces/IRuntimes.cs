using System;
using _build.Models;

namespace _build.Interfaces;

public interface IRuntimes
{
    ReadOnlyMemory<Runtime> Runtimes { get; }
}
