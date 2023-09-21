using Spravy.Service.Extensions;
using Spravy.Service.Middlewares;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Service.Services;
using Spravy.ToDo.Service.Extensions;

WebApplication.CreateBuilder(args)
    .BuildSpravy<GrpcToDoService>(
        args,
        typeof(DataBaseSetupSqliteMiddleware<SpravyToDoDbContext>),
        services => services.AddToDo()
    )
    .Run();