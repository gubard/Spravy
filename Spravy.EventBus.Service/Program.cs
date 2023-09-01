using Spravy.EventBus.Service.Extensions;
using Spravy.EventBus.Service.Services;
using Spravy.Service.Extensions;

WebApplication.CreateBuilder(args).BuildSpravy<GrpcEventBusService>(x => x.AddEventBus()).Run();