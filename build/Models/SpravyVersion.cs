namespace _build.Models;

public struct SpravyVersion
{
    SpravyVersion(ulong major, byte minor, byte build, byte revision)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Revision = revision;
    }

    public SpravyVersion(ulong value)
    {
        var version = new SpravyVersion();

        for (ulong i = 0; i < value; i++)
        {
            version++;
        }

        Major = version.Major;
        Minor = version.Minor;
        Build = version.Build;
        Revision = version.Revision;
    }

    public readonly ulong Major = 1;
    public readonly byte Minor;
    public readonly byte Build;
    public readonly byte Revision;

    public static SpravyVersion operator ++(SpravyVersion version)
    {
        var major = version.Major;
        var minor = version.Minor;
        var build = version.Build;
        var revision = version.Revision;
        revision++;

        if (revision > 20)
        {
            revision = 0;
            build++;
        }

        if (build > 20)
        {
            build = 0;
            minor++;
        }

        if (minor > 20)
        {
            minor = 0;
            major++;
        }

        return new SpravyVersion(major, minor, build, revision);
    }

    public override string ToString() => $"{Major}.{Minor}.{Build}.{Revision}";
}