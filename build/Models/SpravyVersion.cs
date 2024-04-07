namespace _build.Models;

public struct SpravyVersion
{
    public SpravyVersion(ulong major, byte minor, byte build, byte revision)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Revision = revision;
    }

    public readonly ulong Major = 1;
    public readonly byte Minor;
    public readonly byte Build;
    public readonly byte Revision;
    public ulong Code => Major + Minor * 20ul + Build * 20ul + Revision * 20ul;

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

    public static bool TryParse(string str, out SpravyVersion version)
    {
        if (str is null)
        {
            version = new SpravyVersion(1, 0, 0, 0);

            return false;
        }

        var values = str.Split('.');

        if (values.Length != 4)
        {
            version = new SpravyVersion(1, 0, 0, 0);

            return false;
        }

        if (!ulong.TryParse(values[0], out var major))
        {
            version = new SpravyVersion(1, 0, 0, 0);

            return false;
        }

        if (!byte.TryParse(values[1], out var minor))
        {
            version = new SpravyVersion(1, 0, 0, 0);

            return false;
        }

        if (!byte.TryParse(values[2], out var build))
        {
            version = new SpravyVersion(1, 0, 0, 0);

            return false;
        }

        if (!byte.TryParse(values[3], out var revision))
        {
            version = new SpravyVersion(1, 0, 0, 0);

            return false;
        }

        version = new SpravyVersion(major, minor, build, revision);

        return true;
    }
}