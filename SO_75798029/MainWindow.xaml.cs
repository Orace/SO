using System.Text.Json.Nodes;

namespace SO_75798029;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var dataContext = JsonNode.Parse("""
        {
            "true": true,
            "false": false,
            "null": null,
            "number": 69.96,
            "integer": 42,
            "String": "Test",
            "Array": [1, 2, 3, {"P": "Q"}],
            "Inner": {
                "String": "Test"
            }
        }
        """);
        DataContext = new[] { JsonViewModel.From(RootKey.Instance, dataContext) };
    }
}