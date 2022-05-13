using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// read the JSON file. 
var json = JObject.Parse(File.ReadAllText("input.json"));

// Extract label by GUID. Note that GUID case is inconsistent within the file, so we force upper case.
var labelByGuid = json.SelectTokens("cases.procedure.version.forms[*].sections[*].rows[*].fields[?(@.guid && @.label)]")
                      .ToDictionary(t => t["guid"]!.ToString().ToUpperInvariant(),
                                    t => t["label"]!.ToString());

// Update entries.
foreach (var entry in json.SelectTokens("cases.content.[?(@.field)]"))
{
    var field = entry["field"]!.ToString().ToUpperInvariant();
    if (!labelByGuid.TryGetValue(field, out var label))
        continue;

    entry["field"] = label;
}

// write back the JSON object.
var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
var serializer = JsonSerializer.Create(settings);
await using var outputStream = File.CreateText(@"output.json");
serializer.Serialize(outputStream, json);