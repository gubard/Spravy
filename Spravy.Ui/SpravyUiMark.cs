using System.Reflection;

namespace Spravy.Ui;

public readonly struct SpravyUiMark
{
    public static readonly AssemblyName AssemblyName = typeof(SpravyUiMark).Assembly.GetName();
    public static readonly string AssemblyFullName = typeof(SpravyUiMark).Assembly.GetName().FullName;
    public static readonly Assembly Assembly = typeof(SpravyUiMark).Assembly;
}