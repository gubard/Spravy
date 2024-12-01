using Serilog;
using Spravy.Router.Service;
using Spravy.Router.Service.Extensions;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
       .BuildSpravy<GrpcRouterAuthenticationService, GrpcRouterEventBusService, GrpcRouterRouterScheduleService,
            GrpcRouterToDoService, SpravyRouterServiceMark>(args, x => x.RegisterRouter())
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