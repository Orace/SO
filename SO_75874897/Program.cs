using Mapster;

var guid = Guid.NewGuid();

var pocoWithGuid1 = new PocoWithGuid { Id = guid };
Console.WriteLine(pocoWithGuid1.Id);    // 9c29424a-3c7b-4982-b301-84cc7ac41bc6

// Guid to Id fail
var pocoWithId1 = pocoWithGuid1.Adapt<PocoWithId>();
Console.WriteLine(pocoWithId1.Id);      // 00000000-0000-0000-0000-000000000000

var pocoWithId2 = new PocoWithId { Id = new Id(guid) };
Console.WriteLine(pocoWithId2.Id);      // 9c29424a-3c7b-4982-b301-84cc7ac41bc6

// Id to Guid works
var pocoWithGuid2 = pocoWithId2.Adapt<PocoWithGuid>();
Console.WriteLine(pocoWithGuid2.Id);    // 9c29424a-3c7b-4982-b301-84cc7ac41bc6

public class Id
{
    private readonly Guid _guid;

    public Id(Guid id) => _guid = id;

    public static explicit operator Id(Guid value) => new(value);
    public static explicit operator Guid(Id value) => value._guid;

    public override string ToString() => _guid.ToString();
}

public class PocoWithGuid
{
    public Guid Id { get; init; }
}

public class PocoWithId
{
    public Id Id { get; init; }
}