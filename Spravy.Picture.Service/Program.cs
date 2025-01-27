using Serilog;
using Spravy.Domain.Extensions;
using Spravy.Picture.Db.Contexts;
using Spravy.Picture.Service;
using Spravy.Picture.Service.Extensions;
using Spravy.Picture.Service.Services;
using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    Log.Information("Starting web app");

    WebApplication.CreateBuilder(args)
       .BuildSpravy<GrpcPictureService, SpravyPictureMark>(
            args,
            typeof(DataBaseSetupSqliteMiddleware<PictureSpravyDbContext>).ToEnumerable(),
            x => x.RegisterPicture()
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