using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SO_72276645;

public static class LinqRuntimeTypeBuilder
{
    private static readonly Dictionary<string, Type> BuiltTypes;
    private static readonly ModuleBuilder ModuleBuilder;

    static LinqRuntimeTypeBuilder()
    {
        BuiltTypes = new Dictionary<string, Type>();

        var assemblyName = new AssemblyName { Name = "DynamicLinqTypes" };

        ModuleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                                       .DefineDynamicModule(assemblyName.Name);
    }

    private static string GetTypeKey(IReadOnlyDictionary<string, Type> fields)
    {
        var sb = new StringBuilder();
        foreach (var field in fields.OrderBy(f => f.Key))
        {
            sb.Append(field.Key);
            sb.Append(":");
            sb.Append(field.Value.Name);
            sb.Append(";");
        }

        return sb.ToString();
    }

    public static Type GetDynamicType(IReadOnlyDictionary<string, Type> fields)
    {
        if (fields is null)
            throw new ArgumentNullException(nameof(fields));

        if (fields.Count is 0)
            throw new ArgumentOutOfRangeException(nameof(fields), "fields must have at least 1 field definition");

        lock (BuiltTypes)
        {
            var className = GetTypeKey(fields);

            if (BuiltTypes.TryGetValue(className, out var type))
                return type;

            type = CreateDynamicType(className, fields);
            BuiltTypes[className] = type;

            return type;
        }
    }

    private static Type CreateDynamicType(string className, IReadOnlyDictionary<string, Type> fields)
    {
        var typeBuilder = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

        foreach (var field in fields)
        {
            typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public | FieldAttributes.InitOnly);
        }

        return typeBuilder.CreateType();
    }
}