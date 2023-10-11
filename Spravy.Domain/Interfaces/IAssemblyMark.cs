using System.Reflection;

namespace Spravy.Domain.Interfaces;

public interface IAssemblyMark
{
    static abstract AssemblyName AssemblyName { get; }
    static abstract string AssemblyFullName { get; }
    static abstract Assembly Assembly { get; }

    static abstract Stream? GetResourceStream(string resourceName);
}