using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SO_75798029;

public abstract class JsonViewModel
{
    public static JsonViewModel? From(JsonKey key, JsonNode? jsonNode)
    {
        return jsonNode switch
        {
            JsonArray jsonArray => new JsonArrayViewModel(key, jsonArray),
            JsonObject jsonObject => new JsonObjectViewModel(key, jsonObject),
            JsonValue jsonValue => new JsonValueViewModel(key, jsonValue),
            null => new JsonValueViewModel(key, null),
            _ => throw new UnreachableException()
        };
    }

    protected JsonViewModel(JsonKey key)
    {
        Key = key;
    }

    public JsonKey Key { get; }
}

public class JsonArrayViewModel : JsonViewModel
{
    private readonly JsonArray _jsonArray;

    public JsonArrayViewModel(JsonKey key, JsonArray jsonArray) : base(key)
    {
        _jsonArray = jsonArray;
    }

    public IEnumerable<JsonViewModel?> Values => _jsonArray.Select((node, index) => From(new IndexKey(index), node));
}

public class JsonObjectViewModel : JsonViewModel
{
    private readonly JsonObject _jsonObject;

    public JsonObjectViewModel(JsonKey key, JsonObject jsonObject) : base(key)
    {
        _jsonObject = jsonObject;
    }

    public IEnumerable<JsonViewModel?> Values => _jsonObject.Select(kvp => From(new PropertyKey(kvp.Key), kvp.Value));
}

public class JsonValueViewModel : JsonViewModel
{
    public JsonValueViewModel(JsonKey key, JsonValue? jsonValue) : base(key)
    {
        if (jsonValue is null)
        {
            Value = "null";
            ValueKind = JsonValueKind.Null;
        }
        else
        {
            var jsonElement = jsonValue.GetValue<JsonElement>();

            Value = jsonElement.GetRawText();
            ValueKind = jsonElement.ValueKind;
        }
    }

    public string Value { get; }

    public JsonValueKind ValueKind { get; }
}