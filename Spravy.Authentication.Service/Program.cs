using Serilog;
using Spravy.Authentication.Service;
using Spravy.Authentication.Service.Extensions;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
       .BuildSpravy<GrpcAuthenticationService, SpravyAuthenticationServiceMark>(args, x => x.RegisterAuthentication())
       .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}