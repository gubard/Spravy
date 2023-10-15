using Spravy.Router.Service.Extensions;
using Spravy.Router.Service.Services;
using Spravy.Service.Extensions;

WebApplication.CreateBuilder(args).BuildSpravy<GrpcRouterService>(args, x => x.RegisterRouter()).Run();