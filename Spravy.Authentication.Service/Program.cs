using Spravy.Authentication.Service.Extensions;
using Spravy.Authentication.Service.Services;
using Spravy.Service.Extensions;

WebApplication.CreateBuilder(args).BuildSpravy<GrpcAuthenticationService>(args, x => x.RegisterAuthentication()).Run();