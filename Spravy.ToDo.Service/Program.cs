Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
       .BuildSpravy<GrpcToDoService, SpravyToDoServiceMark>(
            args,
            typeof(DataBaseSetupSqliteMiddleware<ToDoSpravyDbContext>),
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