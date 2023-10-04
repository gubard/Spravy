using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Android;

public struct SpravyUiAndroidMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyUiAndroidMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyUiAndroidMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyUiAndroidMark).Assembly;
}