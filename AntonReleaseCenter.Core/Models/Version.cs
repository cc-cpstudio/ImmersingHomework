namespace AntonReleaseCenter.Core.Models;

public record Version(
    int Major,
    int Minor,
    int Build,
    int Revision
) : IComparable<Version>
{
    public int CompareTo(Version? other)
    {
        if (other is null) return 1;

        var cmp = Major.CompareTo(other.Major);
        if (cmp != 0) return cmp;

        cmp = Minor.CompareTo(other.Minor);
        if (cmp != 0) return cmp;

        cmp = Build.CompareTo(other.Build);
        if (cmp != 0) return cmp;

        return Revision.CompareTo(other.Revision);
    }

    public static bool operator >(Version a, Version b) => a.CompareTo(b) > 0;
    public static bool operator <(Version a, Version b) => a.CompareTo(b) < 0;
    public static bool operator >=(Version a, Version b) => a.CompareTo(b) >= 0;
    public static bool operator <=(Version a, Version b) => a.CompareTo(b) <= 0;

    public override string ToString() => $"{Major}.{Minor}.{Build}.{Revision}";

    public static Version Parse(string s)
    {
        var parts = s.Split('.');
        return new Version(
            int.Parse(parts[0]),
            int.Parse(parts[1]),
            int.Parse(parts[2]),
            int.Parse(parts[3])
        );
    }
}
