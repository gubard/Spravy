namespace _build.Models;

public class Runtime
{
    public static readonly Runtime BrowserWasm = new("browser-wasm");
    public static readonly Runtime AndroidArm64 = new("android-arm64");
    public static readonly Runtime LinuxBionicArm64 = new("linux-bionic-arm64");
    public static readonly Runtime LinuxX64 = new("linux-x64");
    public static readonly Runtime WinX64 = new("win-x64");
    public static readonly Runtime WinArm64 = new("win-arm64");
    public static readonly Runtime LinuxArm64 = new("linux-arm64");

    public Runtime(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;
}