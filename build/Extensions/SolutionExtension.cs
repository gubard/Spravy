using System.Collections.Generic;
using System.Linq;
using Nuke.Common.ProjectModel;

namespace _build.Extensions;

public static class SolutionExtension
{
    public static IEnumerable<Project> GetProjects(this Solution solution, string type)
    {
        return solution.AllProjects
            .Where(x => x.Name.EndsWith($".{type}") && x.Name != $"Spravy.{type}");
    }
}