using Avalonia.Headless;
using Spravy.Integration.Tests;

[assembly: Parallelizable(ParallelScope.None)]
[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]