using Spravy.Router.Service.Extensions;
using Spravy.Router.Service.Services;
using Spravy.Service.Extensions;

WebApplication.CreateBuilder(args)
    .BuildSpravy<GrpcAuthenticationService, GrpcEventBusService, GrpcScheduleService, GrpcToDoService>(
        args,
        x => x.RegisterRouter()
    )
    .Run();