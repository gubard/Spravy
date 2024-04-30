using Serilog;
using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Service;
using Spravy.EventBus.Service.Extensions;
using Spravy.EventBus.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
       .BuildSpravy<GrpcEventBusService, SpravyEventBusServiceMark>(args,
            typeof(DataBaseSetupSqliteMiddleware<SpravyDbEventBusDbContext>), x => x.RegisterEventBus())
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