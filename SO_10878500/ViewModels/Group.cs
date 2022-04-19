using System;

namespace SO_10878500.ViewModels;

public class Group : IEquatable<Group>
{
    public Group(string name, bool isRed)
    {
        Name = name;
        IsRed = isRed;
    }

    public string Name { get; }

    public bool IsRed { get; }

    public bool Equals(Group? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && IsRed == other.IsRed;
    }

    public override bool Equals(object? obj) => obj is Group other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Name, IsRed);

    public static bool operator ==(Group? left, Group? right) => Equals(left, right);
    public static bool operator !=(Group? left, Group? right) => !Equals(left, right);
}