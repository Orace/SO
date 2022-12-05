using Newtonsoft.Json;

Try(@"{ ""BoolValue"": true }");
Try(@"{ ""BoolValue"": false }");
Try(@"{ ""BoolValue"": ""Y"" }");
Try(@"{ ""BoolValue"": ""N"" }");
Try(@"{ ""BoolValue"": 10 }");
Try(@"{ ""BoolValue"": ""Z"" }");

Console.ReadLine();

void Try(string json)
{
    var model = JsonConvert.DeserializeObject<Model>(json);
    var content = model is null
                      ? "null model"
                      : model.BoolValue switch
                      {
                          true => "true",
                          false => "false",
                          null => "null"
                      };

    Console.WriteLine(json);
    Console.WriteLine($"Got a {content}");
    Console.WriteLine();
}

public class Model
{
    [JsonConverter(typeof(YNBoolConverter))]
    public bool? BoolValue { get; set; }
}

public class YNBoolConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(value switch
        {
            true => "Y",
            false => "N",
            _ => string.Empty
        });
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return reader.Value switch
        {
            true => true,
            "Y" => true,
            "y" => true,

            false => false,
            "N" => false,
            "n" => false,

            _ => existingValue
        };
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }
}