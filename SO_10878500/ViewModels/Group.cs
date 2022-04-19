namespace SO_10878500.ViewModels;

public class Group
{
    public static Group GroupOfP { get; } = new("Group of P", false);
    public static Group GroupOfG { get; } = new("Group of G", true);

    private Group(string name, bool isRed)
    {
        Name = name;
        IsRed = isRed;
    }

    public string Name { get; }

    public bool IsRed { get; }
}