if (args.Length is 0)
{
    Console.WriteLine("filename missing");
    return;
}

var fileName = args[0];

// Fill the tree
var rootNode = new TreeNode();
foreach (var line in File.ReadLines(fileName))
{
    rootNode.Push(line.Split('/'));
}

// Printing results
Print(rootNode, string.Empty);
return;

void Print(TreeNode treeNode, string path)
{
    foreach (var kvp in treeNode.Children)
    {
        var subPath = path is "" ? kvp.Key : path + '/' + kvp.Key;
        Console.WriteLine($"{subPath}, {kvp.Value.Children.Count}");
        Print(kvp.Value, subPath);
    }
}


public class TreeNode
{
    private readonly Dictionary<string, TreeNode> _children = new();

    public IReadOnlyDictionary<string, TreeNode> Children => _children;

    public void Push(IEnumerable<string> pathParts)
    {
        var currentNode = this;
        foreach (var pathPart in pathParts)
        {
            currentNode = currentNode.GetOrCreateChildNode(pathPart);
        }
    }

    private TreeNode GetOrCreateChildNode(string pathPart)
    {
        if (_children.TryGetValue(pathPart, out var childNode))
            return childNode;

        childNode = new TreeNode();
        _children[pathPart] = childNode;
        return childNode;
    }
}