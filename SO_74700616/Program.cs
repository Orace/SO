using System.Text;

var value = ReadInteger("Please provide a number");
var digits = GetDigits(value);

foreach (var combinaison in GetCombinations(digits))
{
    Console.WriteLine(GetString(combinaison));
}

Console.ReadLine();

static IEnumerable<IReadOnlyList<int>> GetCombinations(Stack<int> stack)
{
    return GetCombinationsImplem(stack, new Stack<int>());
}

static IEnumerable<IReadOnlyList<int>> GetCombinationsImplem(Stack<int> inStack, Stack<int> outStack)
{
    // recursion end point
    if (inStack.Count is 0)
    {
        yield return outStack.Reverse().ToArray();
        yield break;
    }
    
    var digit1 = inStack.Pop();
    
    // Put on digit from in to out
    outStack.Push(digit1);
    foreach (var combinaison in GetCombinationsImplem(inStack, outStack))
    {
        yield return combinaison;
    }
    outStack.Pop();


    // Try put two combined digit (1,2 => 12) from in to out
    if (inStack.TryPop(out var digit2))
    {
        var v = digit1 * 10 + digit2;
        if (v <= 26)
        {
            outStack.Push(v);
            foreach (var combinaison in GetCombinationsImplem(inStack, outStack))
            {
                yield return combinaison;
            }
            outStack.Pop();
        }
        inStack.Push(digit2);
    }

    inStack.Push(digit1);
}

static Stack<int> GetDigits(int value)
{
    var result = new Stack<int>();
    while (value > 0)
    {
        result.Push(value % 10);
        value /= 10;
    }

    return result;
}

static string GetString(IReadOnlyCollection<int> digits)
{
    var sb = new StringBuilder(digits.Count);
    foreach (var digit in digits)
    {
        // Convert 1 to 'a', 2 to 'b', ...
        sb.Append((char)('a' + digit - 1));
    }

    return sb.ToString();
}


static int ReadInteger(string caption)
{
    Console.WriteLine(caption);
    for (;;)
    {
        var line = Console.ReadLine();
        if (line is not null && int.TryParse(line, out var value))
            return value;

        Console.WriteLine("This was not a number.");
    }
}