using System.Text;
using CliWrap;
using Serilog;

namespace _build.Extensions;

public static class CommandExtension
{
    public static void RunCommand(this Command command)
    {
        var errorStringBuilder = new StringBuilder();
        var outputStringBuilder = new StringBuilder();

        command.WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorStringBuilder))
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(outputStringBuilder))
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        Log.Information("{Error}", errorStringBuilder.ToString());
        Log.Information("{Output}", outputStringBuilder.ToString());
    }
}