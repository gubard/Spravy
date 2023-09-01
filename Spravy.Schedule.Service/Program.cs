using Spravy.Schedule.Service.Extensions;
using Spravy.Schedule.Service.Services;
using Spravy.Service.Extensions;

WebApplication.CreateBuilder(args).BuildSpravy<GrpcScheduleService>(x => x.AddSchedule()).Run();