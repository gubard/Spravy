using Serilog;

namespace Spravy.Integration.Tests;

[SetUpFixture]
public class Setup
{
    [OneTimeSetUp]
    public void StartTest()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    [OneTimeTearDown]
    public void EndTest()
    {
        Log.CloseAndFlush();
    }
}