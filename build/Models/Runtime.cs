namespace _build.Models;

public class Runtime
{
    public static readonly Runtime BrowserWasm = new("browser-wasm");
    public static readonly Runtime LinuxX64 = new("linux-x64");
    public static readonly Runtime WinX64 = new("win-x64");

    Runtime(string name)
    {
        Name = name;
    }

    public string Name { get; }
}