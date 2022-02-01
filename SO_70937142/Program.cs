using System.Text;
using System.Windows.Markup;
using System.Windows.Media;

var message = "I  you  💋  嗎";
var fontFamily = new FontFamily("Arial");


foreach (var subString in Split(message))
{
    Print(subString);
}

Console.ReadLine();


string GetFont(int codePoint)
{
    foreach (var typeface in fontFamily.GetTypefaces())
    {
        if (!typeface.TryGetGlyphTypeface(out var glyph))
            continue;

        if (!glyph.CharacterToGlyphMap.ContainsKey(codePoint))
            continue;

        fontFamily.FamilyNames.TryGetValue(XmlLanguage.GetLanguage("en-us"), out var familyName);
        return familyName ?? "Unnamed font";
    }

    return "No matching font";
}

void Print(SubString subString)
{
    var (asString, codePoint) = subString;

    Console.Write(asString.PadRight(2));
    Console.Write(" | ");
    Console.Write(GetFont(codePoint));
    Console.Write(" | ");
    Console.Write($"0x{codePoint:X4}");

    Console.WriteLine();
}

IEnumerable<SubString> Split(string input)
{
    var bytes = Encoding.UTF32.GetBytes(input);
    for (var i = 0; i < bytes.Length; i += 4)
    {
        var codePoint = BitConverter.ToInt32(bytes, i);
        var subString = Encoding.UTF32.GetString(bytes, i, 4);
        yield return new SubString(subString, codePoint);
    }
}

internal record SubString(string AsString, int CodePoint);