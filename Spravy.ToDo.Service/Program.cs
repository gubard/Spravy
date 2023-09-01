using Spravy.Service.Extensions;
using Spravy.ToDo.Service.Services;
using Spravy.ToDo.Service.Extensions;

WebApplication.CreateBuilder(args).BuildSpravy<GrpcToDoService>(services => services.AddToDo()).Run();