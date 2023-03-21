namespace SO_75798029;

public enum JsonKeyType
{
    Root,
    Index,
    Parameter
}

public abstract class JsonKey
{
    public abstract JsonKeyType KeyType { get; }
}

public class IndexKey : JsonKey
{
    public IndexKey(int index)
    {
        Index = index;
    }

    public int Index { get; }

    public override JsonKeyType KeyType => JsonKeyType.Index;

    public override string ToString() => $"@{Index}";
}

public class PropertyKey : JsonKey
{
    public PropertyKey(string key)
    {
        Key = key;
    }

    public string Key { get; }

    public override JsonKeyType KeyType => JsonKeyType.Parameter;

    public override string ToString() => Key;
}

public class RootKey : JsonKey
{
    public static RootKey Instance { get; } = new();

    private RootKey()
    {
    }

    public override JsonKeyType KeyType => JsonKeyType.Root;

    public override string ToString() => "Root";
}