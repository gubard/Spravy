using System.Collections.Generic;
using _build.Extensions;
using _build.Services;

namespace _build.Helpers;

public static class Singleton
{
    static Singleton()
    {
        VersionService = new VersionService("/tmp/Spravy/version.txt".ToFile());

        ProjectBuilderFactory = new ProjectBuilderFactory(new Dictionary<string, ushort>
            {
                {
                    "Spravy.Authentication.Service", 5000
                },
                {
                    "Spravy.EventBus.Service", 5001
                },
                {
                    "Spravy.Router.Service", 5002
                },
                {
                    "Spravy.Schedule.Service", 5003
                },
                {
                    "Spravy.ToDo.Service", 5004
                },
            }
        );
    }

    public static readonly VersionService VersionService;
    public static readonly ProjectBuilderFactory ProjectBuilderFactory;
}