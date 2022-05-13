    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    // read the JSON file. 
    var json = JObject.Parse(File.ReadAllText("input.json"));

    // Extract label by GUID. Note that GUID case is inconsistent within the file, so we force upper case.
    var labelByGuid = json["cases"]["procedure"]["version"]["forms"]
                     .SelectMany(t => t["sections"])
                     .SelectMany(t => t["rows"])
                     .SelectMany(t => t["fields"])
                     .ToDictionary(t => t["guid"].Value<string>().ToUpperInvariant(),
                                   t => t["label"].Value<string>());

    // Update entries.
    foreach (var entry in json["cases"]["content"])
    {
        var field = entry["field"].Value<string>().ToUpperInvariant();
        if (!labelByGuid.TryGetValue(field, out var label))
            continue;
        
        entry["field"] = label;
    }

    // write back the JSON object.
    var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
    var serializer = JsonSerializer.Create(settings);
    await using var outputStream = File.CreateText(@"output.json");
    serializer.Serialize(outputStream, json);