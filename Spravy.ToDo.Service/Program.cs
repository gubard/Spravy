using Serilog;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Service;
using Spravy.ToDo.Service.Services;
using Spravy.ToDo.Service.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
        .BuildSpravy<GrpcToDoService, SpravyToDoServiceMark>(
            args,
            typeof(DataBaseSetupSqliteMiddleware<SpravyDbToDoDbContext>),
            services => services.RegisterToDo()
        )
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